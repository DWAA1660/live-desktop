namespace LiveDesktop;

static class ExtraScenes
{
    public static void Draw(Graphics g, int theme, double time)
    {
        if (theme <= 7) CuteScenes.Draw(g, theme, time);
        else if (theme <= 12) AnimalScenes.Draw(g, theme, time);
        else if (theme <= 17) CoolScenes.Draw(g, theme, time);
        else LivingScenes.Draw(g, theme, time);
    }
}
