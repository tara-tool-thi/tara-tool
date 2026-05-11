
/*Die Infos aus dieser Klasse müssen ins Datenbankschema für Attackpath hinzugefügt werden.
Danach kann diese Klasse gelöscht werden*/
public class AttackPathModel (){
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Label { get; set; } = "AP";
        public List<string> Steps { get; set; } = new();
        public string RiskTreatment { get; set; } = "";
    }