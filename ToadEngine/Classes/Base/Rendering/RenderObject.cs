namespace ToadEngine.Classes.Base.Rendering;

public abstract class RenderObject
{
    private bool _isDisposing, _isSetup;
    public bool IsEnabled = true;

    public virtual void Setup() {}
    public virtual void Draw(float deltaTime) {}
    public virtual void Update(float deltaTime) {}
    public virtual void Resize(FramebufferResizeEventArgs e) {}
    public virtual void Dispose() {}

    public void OnSetup()
    {
        if (_isSetup) return;
        Setup();
        _isSetup = true;
        _isDisposing = false;
    }

    public void OnDraw(float deltaTime)
    {
        if (_isDisposing || !IsEnabled) return;
        Draw(deltaTime);
    }

    public void OnUpdate(float deltaTime)
    {
        if (_isDisposing || !IsEnabled) return;
        Update(deltaTime);
    }

    public void OnResize(FramebufferResizeEventArgs e)
    {
        Resize(e);
    }

    public void OnDispose()
    {
        _isSetup = false;
        _isDisposing = true;
        Dispose();
    }
}
