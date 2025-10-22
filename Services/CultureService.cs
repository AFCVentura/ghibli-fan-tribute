
using Microsoft.JSInterop;

namespace BlazorServerFirstProject.Services
{
    /// <summary>
    /// Serviço que gerencia as culturas.
    /// Para fazer isso, ele utiliza JSInterop para importar funções de um módulo JavaScript. 
    /// Código do projeto chama esse Service -> Service chama JSInterop -> JSInterop chama o JavaScript -> JavaScript altera o tema no DOM.
    /// </summary>
    // IAsyncDisposable serve para liberar o módulo JS corretamente quando o service não for mais usado
    public class CultureService : IAsyncDisposable
    {
        // IJSRuntime é a instância do JSInterop que permite usar funções JS dentro do C#
        private readonly IJSRuntime _js;
        // IJSObjectReference é a instância que representa o módulo JS
        private IJSObjectReference? _module;
        // Construtor recebe o Runtime por injeção de dependência
        public CultureService(IJSRuntime js)
        {
            _js = js;
        }
        // Esse módulo busca o módulo JS e retorna pra quem chamou
        private async Task<IJSObjectReference> ModuleAsync()
        {
            // Se o atributo do módulo já existe é porque ele foi buscado antes, então não precisa fazer de novo (é tipo um cache)
            if (_module is not null) return _module;

            // Essa é a lógica para buscar o módulo, usa o _js e seu método InvokeAsync para usar o "import" no caminho ao lado, sendo o '.', o wwwroot
            _module = await _js.InvokeAsync<IJSObjectReference>("import", "./js/culture.js");
            return _module;
        }

        // Esse método serve para definir uma cultura,
        // ele será chamado quando o usuário selecionar uma cultura na navbar e será passado para ele a cultura como string
        public async Task SetCultureAsync(string culture)
        {
            // Usa o método anterior que busca o módulo JS
            var m = await ModuleAsync();
            // Usa o método do módulo JS para definir a cultura, passando a cultura ao lado
            await m.InvokeVoidAsync("setCulture", culture);
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
