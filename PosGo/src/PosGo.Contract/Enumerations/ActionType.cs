using System.ComponentModel;

namespace PosGo.Contract.Enumerations;

[Flags]
public enum ActionType
{
    [Description("View")]
    View = 1,
    [Description("Add")]
    Add = 2,
    [Description("Update")]
    Update = 4,
    [Description("Delete")]
    Delete = 8,
}
