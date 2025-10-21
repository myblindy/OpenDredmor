namespace OpenDredmor.CommonInterfaces.Support;

public readonly record struct Rect2(float X, float Y, float W, float H)
{
    public bool Contains(float x, float y) =>
        x >= X && x <= X + W && y >= Y && y <= Y + H;
}
