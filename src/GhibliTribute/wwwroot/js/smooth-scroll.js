// Scroll fluido da rodinha do mouse, o mesmo motor do jventura.dev.
// A rodinha nativa anda em degraus fixos por "clique"; aqui cada clique só move um alvo, e a página
// persegue esse alvo a cada frame. Uma rampa curta no começo de cada gesto dá o ease-in, e a
// aproximação exponencial dá a frenagem suave no final. Toque não dispara "wheel", então o celular
// continua com o scroll nativo.
// Não respeita o prefers-reduced-motion de propósito (mesma decisão do jventura.dev): o scroll é a
// sensação do site, não animação decorativa, e muita gente tem essa opção ligada sem saber
// ("Mostrar animações no Windows" desligado ou perfil de energia de melhor desempenho).
(function () {
    var current = window.pageYOffset;
    var target = current;
    var animating = false;
    var rafId = null;
    var gestureStart = null;
    var EASE = 0.085;   // fração do caminho percorrida por frame; menor = mais macio e mais longo
    var RAMP_MS = 220;  // duração do ease-in no começo de cada gesto

    function maxScroll() {
        return document.documentElement.scrollHeight - window.innerHeight;
    }

    function step(now) {
        if (gestureStart === null) gestureStart = now;
        var rampT = Math.min((now - gestureStart) / RAMP_MS, 1);
        current += (target - current) * EASE * rampT * rampT;
        if (Math.abs(target - current) < 0.5) {
            current = target;
            window.scrollTo(0, current);
            animating = false;
            return;
        }
        window.scrollTo(0, current);
        rafId = requestAnimationFrame(step);
    }

    function stop() {
        animating = false;
        gestureStart = null;
        if (rafId !== null) cancelAnimationFrame(rafId);
        current = target = window.pageYOffset;
    }

    window.addEventListener('wheel', function (e) {
        if (e.ctrlKey) return; // zoom com ctrl + rodinha fica com o navegador
        // O Firefox pode mandar o delta em linhas (deltaMode 1) ou páginas (2) em vez de pixels
        var delta = e.deltaY * (e.deltaMode === 1 ? 16 : e.deltaMode === 2 ? window.innerHeight : 1);
        target = Math.max(0, Math.min(target + delta, maxScroll()));
        e.preventDefault();
        if (!animating) {
            animating = true;
            gestureStart = null;
            current = window.pageYOffset;
            rafId = requestAnimationFrame(step);
        }
    }, { passive: false });

    // Scroll vindo de outro lugar (barra de rolagem, teclado, âncora) só atualiza a posição
    window.addEventListener('scroll', function () {
        if (!animating) current = target = window.pageYOffset;
    });

    // Trocar de página no meio de um deslize faria o próximo frame puxar a página nova de volta
    // pro alvo antigo, em vez de o Blazor levar pro topo (ou pra âncora, como em me/techstack#site).
    // O router do Blazor navega por pushState, e o voltar/avançar do navegador dispara popstate.
    var pushState = history.pushState;
    history.pushState = function () {
        stop();
        return pushState.apply(this, arguments);
    };
    window.addEventListener('popstate', stop);
})();
