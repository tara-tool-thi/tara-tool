namespace tara_tool.Data.Services;

public class BreadcrumbCommunicator()
{
    public State currentState { get; set; } = new State();

    public void ClearState()
    {
        //Creates a fresh instance. Grabage collection will handle the rest.
        currentState = new State();
    }
}

public class State()
{
    public KeyValuePair<long, string>? CurrentProject;
    public KeyValuePair<long, string>? CurrentItem;
    public KeyValuePair<long, string>? CurrentAsset;
    public KeyValuePair<long, string>? CurrentDamageScenario;
    public KeyValuePair<long, string>? CurrentThreatScenario;
}