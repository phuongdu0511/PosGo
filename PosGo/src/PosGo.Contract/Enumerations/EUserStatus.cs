namespace PosGo.Contract.Enumerations;

public enum EUserStatus
{
    Active = 1, //start with 1, 0 is used for filter All = 0
    Blocked,
    Locked,
    Pending
}
