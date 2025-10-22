using Microsoft.JSInterop;


namespace BlazorServerFirstProject.Services
{
    /// <summary>
    /// Serviço que gerencia temas claro e escuro.
    /// Para fazer isso, ele utiliza JSInterop para importar funções de um módulo JavaScript. 
    /// Código do projeto chama esse Service -> Service chama JSInterop -> JSInterop chama o JavaScript -> JavaScript altera o tema no DOM.
    /// </summary>
    /// // IAsyncDisposable serve para liberar o módulo JS corretamente quando o service não for mais usado
    public class ThemeService : IAsyncDisposable
    {
        // IJSRuntime é a instância do JSInterop que permite usar funções JS dentro do C#
        private readonly IJSRuntime _js;
        // IJSObjectReference é a instância que representa o módulo JS
        private IJSObjectReference? _module;
        // Construtor recebe o Runtime por injeção de dependência
        public ThemeService(IJSRuntime js)
        {
            _js = js;
        }

        // Esse módulo busca o módulo JS e retorna pra quem chamou
        private async Task<IJSObjectReference> ModuleAsync()
        {
            // Se o atributo do módulo já existe é porque ele foi buscado antes, então não precisa fazer de novo (é tipo um cache)
            if (_module is not null)
                return _module;

            // Essa é a lógica para buscar o módulo, usa o _js e seu método InvokeAsync para usar o "import" no caminho ao lado, sendo o '.', o wwwroot
            _module = await _js.InvokeAsync<IJSObjectReference>("import", "./js/theme.js");
            return _module;
        }

        // Esse método serve para definir um tema, ele será chamado quando o usuário selecionar um tema na navbar e será passado para ele o tema como string
        public async Task SetThemeAsync(string theme)
        {
            // Ele busca o módulo com o método de cima
            var module = await ModuleAsync();
            // Depois usa essa lógica para executar os métodos de set e apply do javascript, os dois precisam receber o tema, ele passa ao lado
            await module.InvokeVoidAsync("setLocalTheme", theme);
            await module.InvokeVoidAsync("applyTheme", theme);
        }

        // Esse método busca o tema salvo no local storage, se não tiver nada, retorna "system"
        public async Task<string> GetThemeAsync()
        {
            // Novamente, busca o módulo JS com o primeiro método
            var module = await ModuleAsync();
            
            // Aqui usa a função do módulo JS para buscar qual o tema salvo no local storage
            string savedTheme = await module.InvokeAsync<string>("getLocalTheme");

            // Aqui usa o IsNullOrWhiteSpace para conferir se tinha algo no local storag
            // se não tinha nada, diz que é system, pois no JS, quando isso ocorre, é verificada a preferência do usuário
            // se tinha algo, é o tema, então retorna ele
            return string.IsNullOrWhiteSpace(savedTheme) ? "system" : savedTheme;
        }

        // Esse método é para inicializar o tema quando a aplicação carrega, ele busca o tema salvo e aplica ele
        public async Task InitAsync()
        {
            // Busca o tema e o módulo
            var theme = await GetThemeAsync();
            var module = await ModuleAsync();

            // Usa o método do módulo JS para aplicar o tema (tem uma função no módulo que faz isso)
            await module.InvokeVoidAsync("applyTheme", theme);
        }

        // Esse método é para liberar o módulo JS quando o service não for mais usado
        public async ValueTask DisposeAsync()
        {
            if (_module is not null)
            {
                await _module.DisposeAsync();
            }
        }
    }
}
