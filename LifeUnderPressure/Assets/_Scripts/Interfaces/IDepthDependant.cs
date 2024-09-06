
public interface IDepthDependant
{
    public bool IDD_OnDepthLevelEnter(int level);
    public void IDD_OnDepthLevelExit(int level);
    public void IDD_NotAllowedUpdate(int level, float deltaTime);
    public int IDD_GetGOInstanceID();
}
