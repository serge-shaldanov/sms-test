using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SmsTest.VariablesApp.Models.Entities;
using SmsTest.VariablesApp.Models.Services;
using SmsTest.VariablesApp.ViewModels.Events;

namespace SmsTest.VariablesApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IVariablesService _variables;

    public MainViewModel(IVariablesService variablesService)
    {
        ArgumentNullException.ThrowIfNull(variablesService);
        this._variables = variablesService;

        this.Items = [];
    }

    public ObservableCollection<VariableItemViewModel> Items { get; }

    [RelayCommand]
    private void Load()
    {
        // Развитие:
        // 1) Собственный ViewManager с хуками времени жизни (InitializeAsync / SetupAsync / DisposeAsync / ...),
        // чтобы не плодить аналогичный код из компонента в компонент.
        // 2) Переход на Messenger со слабыми ссылками на дочерние компоненты (сейчас время жизни окна = времени жизни
        // приложения, поэтому утечек нет).
        IEnumerable<Variable> availableVariables = this._variables.GetVariables();

        foreach (Variable variable in availableVariables)
        {
            VariableItemViewModel itemViewModel = new VariableItemViewModel(variable);
            itemViewModel.VariableChanged += this.OnVariableChanged;

            this.Items.Add(itemViewModel);
        }
    }

    private void OnVariableChanged(object? sender, VariableChangedEventArgs e)
    {
        // Развитие:
        // Написание BackgroundTaskRunner для фоновых задач, чтобы не нагружать основной поток + использовать
        // единообразный интерфейс ожидания операции + использовать глобальный перехват ошибок
        this._variables.UpdateVariable(e.Variable);
    }
}
