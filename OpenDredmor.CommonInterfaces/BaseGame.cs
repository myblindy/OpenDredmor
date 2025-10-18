namespace OpenDredmor.CommonInterfaces;

public abstract class BaseGame(BaseVFS vfs, BaseRenderer renderer, TimeProvider timeProvider)
{
    protected BaseVFS VFS => vfs;
    protected TimeProvider TimeProvider => timeProvider;
    protected BaseRenderer Renderer => renderer;

    public void Run() => Renderer.Run();
}
