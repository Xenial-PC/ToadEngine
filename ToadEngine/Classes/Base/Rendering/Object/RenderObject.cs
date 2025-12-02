namespace ToadEngine.Classes.Base.Rendering.Object;

public abstract class RenderObject
{
    private bool _isDisposing, _isSetup;
    public bool IsEnabled = true;

    public virtual void Setup() { }
    public virtual void Draw() { }
    public virtual void Update() { }
    public virtual void Resize(FramebufferResizeEventArgs e) { }
    public virtual void Dispose() { }

    public void OnSetup()
    {
        if (_isSetup) return;
        Setup();
        _isSetup = true;
        _isDisposing = false;
    }

    public void OnDraw()
    {
        if (_isDisposing || !IsEnabled) return;
        Draw();
    }

    public void OnUpdate()
    {
        if (_isDisposing || !IsEnabled) return;
        Update();
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
