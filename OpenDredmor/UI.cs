using OpenDredmor.CommonInterfaces;
using OpenDredmor.CommonInterfaces.Support;

namespace OpenDredmor;

sealed class UI : BaseUI
{
    float x, y;
    bool down, lastFrameDown, upThisFrame;

    public UI(BaseRenderer renderer) : base(renderer)
    {
        Renderer.OnNewFrame += () =>
        {
            upThisFrame = !down && lastFrameDown;
            lastFrameDown = down;
        };
    }

    protected override void OnMouseMoved(object? sender, BaseRenderer.MouseMovedEventArgs e) =>
        (x, y) = (e.X, e.Y);

    protected override void OnMouseAction(object? sender, BaseRenderer.MouseActionEventArgs e) =>
        (x, y, down) = (e.X, e.Y, e.Pressed);

    public override bool ImageButton(string image, string? imageHover, string? imageDown, in Rect2 rect, SpriteAnchor anchor, int expansion)
    {
        var transformedRect = Renderer.TransformRect2(image, rect, anchor);
        var inside = transformedRect.Contains(x, y);
        Renderer.RenderSprites(new Sprite(
            inside && down && imageDown is not null ? imageDown
                : inside && !down && imageHover is not null ? imageHover
                : image,
            rect, Anchor: anchor));

        return inside && upThisFrame;
    }
}
