using OpenDredmor.CommonInterfaces.Support;

namespace OpenDredmor.CommonInterfaces.Services.Interfaces;

public abstract class BaseUI
{
    protected BaseRenderer Renderer { get; }

    public bool IsLastHovered { get; protected set; }

    public BaseUI(BaseRenderer renderer)
    {
        Renderer = renderer;

        Renderer.OnMouseAction += OnMouseAction;
        Renderer.OnMouseMoved += OnMouseMoved;
    }

    protected abstract void OnMouseMoved(object? sender, BaseRenderer.MouseMovedEventArgs e);
    protected abstract void OnMouseAction(object? sender, BaseRenderer.MouseActionEventArgs e);

    public abstract bool InvisibleButton(in Rect2 rect, SpriteAnchor anchor = SpriteAnchor.TopLeft, string? referenceImage = null);
    public abstract bool ImageButton(string image, string? imageHover, string? imageDown, in Rect2 rect,
        SpriteAnchor anchor = SpriteAnchor.TopLeft, int expansion = -1);
}
