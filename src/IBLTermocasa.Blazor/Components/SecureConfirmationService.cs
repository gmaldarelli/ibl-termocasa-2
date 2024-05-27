using System;
using System.Threading.Tasks;
using Blazorise;
using IBLTermocasa.Localization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace IBLTermocasa.Blazor.Components
{
    public class SecureConfirmationService
    {
        private IStringLocalizer L;
        public IServiceProvider ServiceProvider { get; }
        [Inject] public IModalService ModalService { get; set; }

        private TaskCompletionSource<bool> _taskCompletionSource;

        public SecureConfirmationService(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            IStringLocalizerFactory? stringLocalizerFactory = ServiceProvider.GetService<IStringLocalizerFactory>();
            L = stringLocalizerFactory.Create<IBLTermocasaResource>();
        }

        public Task<bool> ShowConfirmation(string title, string message, string matchValue)
        {
            return ShowConfirmation(title, message, matchValue, null);
        }

        public Task<bool> ShowConfirmation(string title, string message, string matchValue, ModalInstanceOptions? modalInstanceOptions)
        {
            _taskCompletionSource = new TaskCompletionSource<bool>();

            var normalizedMessage = message.Replace("{0}", $"<{matchValue}>");

            void Parameters(ModalProviderParameterBuilder<SecureConfirmation> builder)
            {
                builder.Add(p => p.Title, title);
                builder.Add(p => p.Message, normalizedMessage);
                builder.Add(p => p.MatchValue, matchValue);
                builder.Add(p => p.OnConfirmCallback, (Action)OnConfirmationChanged);
                builder.Add(p => p.OnCancelCallback, (Action)OnConfirmationCancelChanged);
            }

            if (modalInstanceOptions == null)
            {
                modalInstanceOptions = new ModalInstanceOptions()
                {
                    Animated = false,
                    UseModalStructure = true,
                    Size = ModalSize.Default,
                    Centered = true,
                    ShowBackdrop = true,
                };
            }

            ModalService.Show<SecureConfirmation>(L["SecureDeletionTitle"], Parameters, modalInstanceOptions);

            return _taskCompletionSource.Task;
        }

        private void OnConfirmationChanged()
        {
            if (_taskCompletionSource.Task.IsCompleted == false)
            {
                _taskCompletionSource.SetResult(true);
            }
        }

        private void OnConfirmationCancelChanged()
        {
            if (_taskCompletionSource.Task.IsCompleted == false)
            {
                _taskCompletionSource.SetResult(false);
            }
        }
    }
}
