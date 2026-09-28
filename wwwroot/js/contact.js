// Comportamentos da página de contato. Ela é estática (SSR), então não tem C# rodando no navegador:
// o que é interação pura de tela fica aqui.
(function () {
    // Botão de copiar o e-mail: troca o texto por "Copiado!" por 2 segundos
    document.addEventListener('click', function (e) {
        var button = e.target.closest('[data-copy]');
        if (!button) return;
        navigator.clipboard.writeText(button.dataset.copy).then(function () {
            var label = button.querySelector('[data-copy-label]');
            var original = label.textContent;
            label.textContent = button.dataset.copied;
            setTimeout(function () { label.textContent = original; }, 2000);
        });
    });

    // Evita envio duplicado com clique duplo ou impaciência enquanto o servidor responde
    document.addEventListener('submit', function (e) {
        if (e.target.id !== 'contact-form') return;
        var button = e.target.querySelector('button[type="submit"]');
        // Deixa o navegador montar o POST antes de desabilitar
        setTimeout(function () {
            button.disabled = true;
            button.textContent = button.dataset.sending;
        }, 0);
    });

    // Chamada pelo script do Turnstile quando carrega (onload=ghibliTurnstileLoad). A renderização é manual
    // pra seguir o tema escolhido no site, e não só o do sistema. O widget coloca um campo escondido
    // "cf-turnstile-response" dentro do formulário, que o servidor confere com a Cloudflare.
    window.ghibliTurnstileLoad = function () {
        var container = document.getElementById('contact-turnstile');
        if (!container) return;
        var widgetId = turnstile.render(container, {
            sitekey: container.dataset.sitekey,
            language: container.dataset.language,
            theme: document.documentElement.classList.contains('dark') ? 'dark' : 'light'
        });

        // Saindo da página pelo menu, o Blazor troca o conteúdo sem recarregar (navegação aprimorada) e o
        // widget some do DOM. Sem remover, o script da Cloudflare fica procurando por ele e enchendo o console.
        // O script do Turnstile é async e pode terminar antes do blazor.web.js (que vem no fim do body) existir
        function watchNavigation() {
            Blazor.addEventListener('enhancedload', function cleanup() {
                if (document.getElementById('contact-turnstile')) return;
                turnstile.remove(widgetId);
                Blazor.removeEventListener('enhancedload', cleanup);
            });
        }
        if (window.Blazor) watchNavigation();
        else document.addEventListener('DOMContentLoaded', watchNavigation);
    };
})();
