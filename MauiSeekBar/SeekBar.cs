namespace MauiSeekBar;

public class SeekBar : GraphicsView, IDrawable
{
    private const int SeekLinesCount = 100;

    public SeekBar()
    {
        Drawable = this;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // TODO debug
        //canvas.FillColor = Colors.LightGray;
        //canvas.FillRectangle(0, 0, (float)Width, (float)Height);

        DrawSeekLines(canvas, 5, 40, (float)Width - 10, 21);
        DrawPositionMarker(canvas, 0, 55, 10, 21);
        DrawStartMarker(canvas, 0, 25, 10, 15);
        DrawEndMarker(canvas, 100, 25, 10, 15);
    }

    private static void DrawSeekLines(ICanvas canvas, float x, float y, float w, float h)
    {
        float linesSpacing = w / (SeekLinesCount - 1);
        for (int i = 0; i < SeekLinesCount; i++)
        {
            canvas.StrokeColor = Colors.Black;
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

        canvas.FillColor = Colors.White;
        canvas.FillPath(path);

        canvas.StrokeColor = Colors.Black;
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

        canvas.FillColor = Color.FromArgb("#ffffbbbb");
        canvas.FillPath(path);

        canvas.StrokeColor = Color.FromArgb("#ffff7777");
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

        canvas.FillColor = Color.FromArgb("#ffffbbbb");
        canvas.FillPath(path);

        canvas.StrokeColor = Color.FromArgb("#ffff7777");
        canvas.StrokeSize = 1;
        canvas.DrawPath(path);
    }
}
