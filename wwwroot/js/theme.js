// Vamos ter alguns métodos aqui
/*
    applyTheme -> Simplesmente recebe um tema e aplica ele no DOM
    setLocalTheme -> Recebe um tema e salva ele no localStorage
    getLocalTheme -> Retorna o tema salvo no localStorage
*/
// Todos tem export pois a gente importa dentro de um service C# usando um cara chamado JSInterop

// Esse primeiro método só recebe um tema e aplica ele no DOM.
// Lembrando que aplicar consiste em adicionar uma classe na tag <html>
export function applyTheme(theme) {
    // Coleta a tag <html>
    const html = document.documentElement;

    // Confere o tema recebido e adiciona ou remove a classe dark
    if (theme === 'dark') {
        html.classList.add('dark');
    }
    else if (theme === 'light') {
        html.classList.remove('dark');
    } 
    else {
        // Se não houver especificação, confere qual a preferência do usuário por modo dark
        // Essa variável será um boolean
        const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;

        // Com base no boolean acima, usa o toggle
        // Esse toggle vai colocar o 'dark' se a variável for true e vai remover se for false
        html.classList.toggle('dark', prefersDark);
    }
}

// Recebe um tema e salva no localstorage
export function setLocalTheme(theme) {
    localStorage.setItem('theme', theme);
}

// Busca o tema salvo no localstorage
export function getLocalTheme() {
    return localStorage.getItem('theme');
}
