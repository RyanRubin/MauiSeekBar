namespace MauiSeekBar;

public class SeekBar : GraphicsView, IDrawable
{
    private const int SeekLinesCount = 100;

    private static readonly Color StartAndEndFillColor = Color.FromArgb("#ffffbbbb");
    private static readonly Color StartAndEndStrokeColor = Color.FromArgb("#ffff7777");
    private static readonly Color SeekLinesStrokeColor = Colors.Black;
    private static readonly Color PositionMarkerFillColor = Colors.White;
    private static readonly Color PositionMarkerStrokeColor = Colors.Black;

    public SeekBar()
    {
        Drawable = this;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // TODO debug
        //canvas.FillColor = Colors.LightGray;
        //canvas.FillRectangle(0, 0, (float)Width, (float)Height);

        DrawStartToEndBar(canvas, 10, 43, 90, 15);
        DrawSeekLines(canvas, 5, 40, (float)Width - 10, 21);
        DrawPositionMarker(canvas, 0, 58, 10, 21);
        DrawStartMarker(canvas, 0, 28, 10, 15);
        DrawEndMarker(canvas, 100, 28, 10, 15);
    }

    private static void DrawStartToEndBar(ICanvas canvas, float x, float y, float w, float h)
    {
        canvas.FillColor = StartAndEndFillColor;
        canvas.FillRectangle(x, y, w, h);

        canvas.StrokeColor = StartAndEndStrokeColor;
        canvas.StrokeSize = 1;
        canvas.DrawRectangle(x, y, w, h);
    }

    private static void DrawSeekLines(ICanvas canvas, float x, float y, float w, float h)
    {
        float linesSpacing = w / (SeekLinesCount - 1);
        for (int i = 0; i < SeekLinesCount; i++)
        {
            canvas.StrokeColor = SeekLinesStrokeColor;
            if (i == 0 || i == SeekLinesCount - 1)
            {
                canvas.StrokeSize = 3;
                canvas.DrawLine(
                    x + i * linesSpacing,
                    y,
                    x + i * linesSpacing,
                    y + h
                );
            }
            else
            {
                canvas.StrokeSize = 1;
                canvas.DrawLine(
                    x + i * linesSpacing,
                    y + 3,
                    x + i * linesSpacing,
                    y + h - 3
                );
            }
        }
    }

    private static void DrawPositionMarker(ICanvas canvas, float x, float y, float w, float h)
    {
        var path = new PathF();

        path.MoveTo(x + w / 2, y);
        path.LineTo(x + w, y + 5);
        path.LineTo(x + w, y + h);

        path.LineTo(x, y + h);
        path.LineTo(x, y + 5);
        path.LineTo(x + w / 2, y);

        canvas.FillColor = PositionMarkerFillColor;
        canvas.FillPath(path);

        canvas.StrokeColor = PositionMarkerStrokeColor;
        canvas.StrokeSize = 1;
        canvas.DrawPath(path);
    }

    private static void DrawStartMarker(ICanvas canvas, float x, float y, float w, float h)
    {
        var path = new PathF();

        path.MoveTo(x + w, y);
        path.LineTo(x + w, y + h);

        path.LineTo(x, y + h);
        path.LineTo(x + w, y);

        canvas.FillColor = StartAndEndFillColor;
        canvas.FillPath(path);

        canvas.StrokeColor = StartAndEndStrokeColor;
        canvas.StrokeSize = 1;
        canvas.DrawPath(path);
    }

    private static void DrawEndMarker(ICanvas canvas, float x, float y, float w, float h)
    {
        var path = new PathF();

        path.MoveTo(x, y);
        path.LineTo(x + w, y + h);

        path.LineTo(x, y + h);
        path.LineTo(x, y);

        canvas.FillColor = StartAndEndFillColor;
        canvas.FillPath(path);

        canvas.StrokeColor = StartAndEndStrokeColor;
        canvas.StrokeSize = 1;
        canvas.DrawPath(path);
    }
}
