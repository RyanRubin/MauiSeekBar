using Font = Microsoft.Maui.Graphics.Font;

namespace MauiSeekBar;

public class SeekBar : GraphicsView, IDrawable
{
    private const float TicksX = 13;
    private const int TicksCount = 101; // 0 to 100
    private const float PositionMarkerWidth = 10;
    private const float PositionMarkerHalfWidth = PositionMarkerWidth / 2;
    private const float PositionTextWidth = 95;
    private const float PositionTextHalfWidth = PositionTextWidth / 2;
    private const float StartMarkerWidth = 13;
    private const float EndMarkerWidth = 13;

    private static readonly Color StartAndEndFillColor = Color.FromArgb("#ffffbbbb");
    private static readonly Color StartAndEndStrokeColor = Colors.DarkRed;
    private static readonly Color TicksStrokeColor = Colors.Black;
    private static readonly Color PositionMarkerFillColor = Colors.White;
    private static readonly Color PositionMarkerStrokeColor = Colors.Black;
    private static readonly Color PositionTextFillColor = Color.FromArgb("#ffeeeeee");
    private static readonly Color PositionTextStrokeColor = Colors.Gray;
    private static readonly Color PositionTextFontColor = Colors.Gray;

    private bool isSkipMovePositionMarker;
    private float ticksWidth;
    private float positionMarkerX = TicksX - PositionMarkerHalfWidth;
    private float positionTextX = TicksX;
    private float startMarkerX = TicksX - StartMarkerWidth;
    private float endMarkerX = TicksX;

    public static readonly BindableProperty IsStartAndEndMarkerVisibleProperty = BindableProperty.Create(nameof(IsStartAndEndMarkerVisible), typeof(bool), typeof(SeekBar), default, BindingMode.TwoWay, propertyChanged: OnIsStartAndEndMarkerVisibleChanged);
    public bool IsStartAndEndMarkerVisible { get => (bool)GetValue(IsStartAndEndMarkerVisibleProperty); set => SetValue(IsStartAndEndMarkerVisibleProperty, value); }
    private static void OnIsStartAndEndMarkerVisibleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var seekBar = (SeekBar)bindable;
        seekBar.Invalidate();
    }

    public static readonly BindableProperty IsPositionTextVisibleProperty = BindableProperty.Create(nameof(IsPositionTextVisible), typeof(bool), typeof(SeekBar), default, BindingMode.TwoWay, propertyChanged: OnIsPositionTextVisibleChanged);
    public bool IsPositionTextVisible { get => (bool)GetValue(IsPositionTextVisibleProperty); set => SetValue(IsPositionTextVisibleProperty, value); }
    private static void OnIsPositionTextVisibleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var seekBar = (SeekBar)bindable;
        seekBar.Invalidate();
    }

    public static readonly BindableProperty IsPlayingProperty = BindableProperty.Create(nameof(IsPlaying), typeof(bool), typeof(SeekBar), default, BindingMode.TwoWay);
    public bool IsPlaying { get => (bool)GetValue(IsPlayingProperty); set => SetValue(IsPlayingProperty, value); }

    public static readonly BindableProperty PositionProperty = BindableProperty.Create(nameof(Position), typeof(TimeSpan), typeof(SeekBar), default, BindingMode.TwoWay, propertyChanged: OnPositionChanged);
    public TimeSpan Position { get => (TimeSpan)GetValue(PositionProperty); set => SetValue(PositionProperty, value); }
    private static void OnPositionChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var seekBar = (SeekBar)bindable;
        seekBar.MovePositionMarker(seekBar.CalculateMarkerX(seekBar.Position));
        seekBar.Invalidate();
    }

    public static readonly BindableProperty DurationProperty = BindableProperty.Create(nameof(Duration), typeof(TimeSpan), typeof(SeekBar), default, BindingMode.TwoWay, propertyChanged: OnDurationChanged);
    public TimeSpan Duration { get => (TimeSpan)GetValue(DurationProperty); set => SetValue(DurationProperty, value); }
    private static void OnDurationChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var seekBar = (SeekBar)bindable;
        seekBar.End = seekBar.Duration;
        seekBar.MovePositionMarker(seekBar.CalculateMarkerX(seekBar.Position));
        seekBar.MoveStartMarker(seekBar.CalculateMarkerX(seekBar.Start));
        seekBar.MoveEndMarker(seekBar.CalculateMarkerX(seekBar.End));
        seekBar.Invalidate();
    }

    public static readonly BindableProperty StartProperty = BindableProperty.Create(nameof(Start), typeof(TimeSpan), typeof(SeekBar), default, BindingMode.TwoWay, propertyChanged: OnStartChanged);
    public TimeSpan Start { get => (TimeSpan)GetValue(StartProperty); set => SetValue(StartProperty, value); }
    private static void OnStartChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var seekBar = (SeekBar)bindable;
        if (seekBar.Start > seekBar.End)
        {
            seekBar.End = seekBar.Start;
        }
        seekBar.MoveStartMarker(seekBar.CalculateMarkerX(seekBar.Start));
        seekBar.Invalidate();
    }

    public static readonly BindableProperty EndProperty = BindableProperty.Create(nameof(End), typeof(TimeSpan), typeof(SeekBar), default, BindingMode.TwoWay, propertyChanged: OnEndChanged);
    public TimeSpan End { get => (TimeSpan)GetValue(EndProperty); set => SetValue(EndProperty, value); }
    private static void OnEndChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var seekBar = (SeekBar)bindable;
        if (seekBar.End < seekBar.Start)
        {
            seekBar.Start = seekBar.End;
        }
        seekBar.MoveEndMarker(seekBar.CalculateMarkerX(seekBar.End));
        seekBar.Invalidate();
    }

    private float CalculateMarkerX(TimeSpan markerTimeSpan)
    {
        float markerTimeSpanMillis = (float)markerTimeSpan.TotalMilliseconds;
        float durationMillis = (float)Duration.TotalMilliseconds;
        float retVal;
        if (durationMillis == 0)
        {
            retVal = 0;
        }
        else
        {
            retVal = markerTimeSpanMillis / durationMillis * ticksWidth + TicksX;
        }
        return retVal;
    }

    private TimeSpan CalculatePosition()
    {
        float durationMillis = (float)Duration.TotalMilliseconds;
        TimeSpan retVal;
        if (durationMillis == 0)
        {
            retVal = default;
        }
        else
        {
            float positionMillis = (positionMarkerX - TicksX + PositionMarkerHalfWidth) / ticksWidth * durationMillis;
            retVal = new TimeSpan(0, 0, 0, 0, (int)positionMillis);
        }
        return retVal;
    }

    public SeekBar()
    {
        Drawable = this;
        StartInteraction += SeekBar_StartInteraction;
        DragInteraction += SeekBar_DragInteraction;
    }

    private void SeekBar_StartInteraction(object? sender, TouchEventArgs e)
    {
        HandleStartAndDragInteraction(e.Touches[0].X);
    }

    private void SeekBar_DragInteraction(object? sender, TouchEventArgs e)
    {
        HandleStartAndDragInteraction(e.Touches[0].X);
    }

    private void HandleStartAndDragInteraction(float x)
    {
        IsPlaying = false;
        MovePositionMarker(x);
        isSkipMovePositionMarker = true;
        Position = CalculatePosition();
        isSkipMovePositionMarker = false;
    }

    private void MovePositionMarker(float x)
    {
        if (isSkipMovePositionMarker)
        {
            return;
        }

        positionMarkerX = x - PositionMarkerHalfWidth;
        positionMarkerX = Math.Clamp(
            positionMarkerX,
            TicksX - PositionMarkerHalfWidth,
            TicksX + ticksWidth - PositionMarkerHalfWidth
        );

        positionTextX = x - PositionTextHalfWidth;
        positionTextX = Math.Clamp(
            positionTextX,
            TicksX,
            TicksX + ticksWidth - PositionTextWidth
        );

        Invalidate();
    }

    private void MoveStartMarker(float x)
    {
        startMarkerX = x - StartMarkerWidth;
        startMarkerX = Math.Clamp(
            startMarkerX,
            TicksX - StartMarkerWidth,
            TicksX + ticksWidth - StartMarkerWidth
        );

        Invalidate();
    }

    private void MoveEndMarker(float x)
    {
        endMarkerX = x;
        endMarkerX = Math.Clamp(
            endMarkerX,
            TicksX,
            TicksX + ticksWidth
        );

        Invalidate();
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (IsStartAndEndMarkerVisible)
        {
            DrawStartToEndBar(canvas, startMarkerX + StartMarkerWidth, 33, endMarkerX - startMarkerX - StartMarkerWidth, 15);
        }
        ticksWidth = (float)Width - TicksX * 2;
        DrawTicks(canvas, TicksX, 30, ticksWidth, 21);
        DrawPositionMarker(canvas, positionMarkerX, 48, PositionMarkerWidth, 19);
        if (IsStartAndEndMarkerVisible)
        {
            DrawStartMarker(canvas, startMarkerX, 20, StartMarkerWidth, 13);
            DrawEndMarker(canvas, endMarkerX, 20, EndMarkerWidth, 13);
        }
        if (IsPositionTextVisible)
        {
            DrawPositionText(canvas, positionTextX, 0, PositionTextWidth, 20);
        }
    }

    private static void DrawStartToEndBar(ICanvas canvas, float x, float y, float w, float h)
    {
        canvas.FillColor = StartAndEndFillColor;
        canvas.FillRectangle(x, y, w, h);

        canvas.StrokeColor = StartAndEndStrokeColor;
        canvas.StrokeSize = 1;
        canvas.DrawRectangle(x, y, w, h);
    }

    private static void DrawTicks(ICanvas canvas, float x, float y, float w, float h)
    {
        float ticksSpacing = w / (TicksCount - 1);
        for (int i = 0; i < TicksCount; i++)
        {
            canvas.StrokeColor = TicksStrokeColor;
            if (i == 0 || i == TicksCount - 1)
            {
                canvas.StrokeSize = 3;
                canvas.DrawLine(
                    x + i * ticksSpacing,
                    y,
                    x + i * ticksSpacing,
                    y + h
                );
            }
            else
            {
                canvas.StrokeSize = 1;
                canvas.DrawLine(
                    x + i * ticksSpacing,
                    y + 3,
                    x + i * ticksSpacing,
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
            Position.ToString("hh\\:mm\\:ss\\.fff"),
            x + 5,
            y,
            w - 10,
            h,
            HorizontalAlignment.Left,
            VerticalAlignment.Top
        );
    }
}
