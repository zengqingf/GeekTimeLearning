


namespace Tenmove.Runtime.EmbedUI
{
    public delegate void UIAction(int actionID);
    public delegate void UIAction<T>(int actionID,T param);
    public delegate void UIAction<T0, T1>(int actionID, T0 param0, T1 param1);
    public delegate void UIAction<T0, T1, T2>(int actionID, T0 param0, T1 param1, T2 param2);
}