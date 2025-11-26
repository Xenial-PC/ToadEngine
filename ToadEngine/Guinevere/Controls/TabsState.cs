namespace Guinevere;

internal class TabsState
{
    public int ActiveTabIndex { get; set; }
    public List<TabInfo> Tabs { get; set; } = [];
    public float TabBarHeight { get; set; } = 32;
}
