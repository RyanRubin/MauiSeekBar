using Font = Microsoft.Maui.Graphics.Font;

namespace MauiSeekBar;

public class SeekBar : GraphicsView, IDrawable
{
    private const float SeekLinesX = 13;
    private const int SeekLinesCount = 100;
    private const float PositionMarkerWidth = 10;
    private const float PositionMarkerHalfWidth = PositionMarkerWidth / 2;
    private const float PositionTextWidth = 95;
    private const float PositionTextHalfWidth = PositionTextWidth / 2;

    private static readonly Color StartAndEndFillColor = Color.FromArgb("#ffffbbbb");
    private static readonly Color StartAndEndStrokeColor = Colors.DarkRed;
    private static readonly Color SeekLinesStrokeColor = Colors.Black;
    private static readonly Color PositionMarkerFillColor = Colors.White;
    private static readonly Color PositionMarkerStrokeColor = Colors.Black;
    private static readonly Color PositionTextFillColor = Color.FromArgb("#ffeeeeee");
    private static readonly Color PositionTextStrokeColor = Colors.Gray;
    private static readonly Color PositionTextFontColor = Colors.Gray;

    private float seekLinesWidth;
    private float positionMarkerX = SeekLinesX - PositionMarkerHalfWidth;
    private float positionTextX = SeekLinesX;

    public static readonly BindableProperty IsStartAndEndMarkerVisibleProperty = BindableProperty.Create(nameof(IsStartAndEndMarkerVisible), typeof(bool), typeof(SeekBar), default, BindingMode.TwoWay);
    public bool IsStartAndEndMarkerVisible { get => (bool)GetValue(IsStartAndEndMarkerVisibleProperty); set => SetValue(IsStartAndEndMarkerVisibleProperty, value); }

    public static readonly BindableProperty PositionProperty = BindableProperty.Create(nameof(Position), typeof(TimeSpan), typeof(SeekBar), default, BindingMode.TwoWay, propertyChanged: OnPositionChanged);
    public TimeSpan Position { get => (TimeSpan)GetValue(PositionProperty); set => SetValue(PositionProperty, value); }
    private static void OnPositionChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var seekBar = (SeekBar)bindable;
        seekBar.Invalidate();
    }

    public SeekBar()
    {
        Drawable = this;
        StartInteraction += SeekBar_StartInteraction;
        DragInteraction += SeekBar_DragInteraction;
        EndInteraction += SeekBar_EndInteraction;
    }

    private void SeekBar_StartInteraction(object? sender, TouchEventArgs e)
    {
        MovePositionMarker(e.Touches[0].X);
    }

    private void SeekBar_DragInteraction(object? sender, TouchEventArgs e)
    {
        MovePositionMarker(e.Touches[0].X);
    }

    private void SeekBar_EndInteraction(object? sender, TouchEventArgs e)
    {

    }

    private void MovePositionMarker(float x)
    {
        positionMarkerX = x - PositionMarkerHalfWidth;
        positionMarkerX = Math.Clamp(
            positionMarkerX,
            SeekLinesX - PositionMarkerHalfWidth,
            SeekLinesX + seekLinesWidth - PositionMarkerHalfWidth
        );

        positionTextX = x - PositionTextHalfWidth;
        positionTextX = Math.Clamp(
            positionTextX,
            SeekLinesX,
            SeekLinesX + seekLinesWidth - PositionTextWidth
        );

        Invalidate();
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (IsStartAndEndMarkerVisible)
        {
            DrawStartToEndBar(canvas, 13, 33, 87, 15);
        }
        seekLinesWidth = (float)Width - SeekLinesX * 2;
        DrawSeekLines(canvas, SeekLinesX, 30, seekLinesWidth, 21);
        DrawPositionMarker(canvas, positionMarkerX, 48, PositionMarkerWidth, 19);
        if (IsStartAndEndMarkerVisible)
        {
            DrawStartMarker(canvas, 0, 20, 13, 13);
            DrawEndMarker(canvas, 100, 20, 13, 13);
        }
        DrawPositionText(canvas, positionTextX, 0, PositionTextWidth, 20);
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

    private void DrawPositionText(ICanvas canvas, float x, float y, float w, float h)
    {
        canvas.FillColor = PositionTextFillColor;
        canvas.FillRoundedRectangle(x, y, w, h, 3);

        canvas.StrokeColor = PositionTextStrokeColor;
        canvas.StrokeSize = 1;
        canvas.DrawRoundedRectangle(x, y, w, h, 3);

        canvas.FontColor = PositionTextFontColor;
        canvas.FontSize = 15;
        canvas.Font = Font.Default;
        canvas.DrawString(
            Position.ToString(@"hh\:mm\:ss\.fff"),
            x + 5,
            y,
            w - 10,
            h,
            HorizontalAlignment.Left,
            VerticalAlignment.Top
        );
    }
}
