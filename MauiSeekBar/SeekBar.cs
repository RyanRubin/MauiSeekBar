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
        canvas.FillColor = Colors.LightGray;
        canvas.FillRectangle(0, 0, (float)Width, (float)Height);

        DrawSeekLines(canvas, 5, 40, (float)Width - 10, 21);
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
}
