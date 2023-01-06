public interface IGameViewElement
{
    public void Activate(ColorScheme colorScheme);

    public void ChangeColor(ColorScheme colorScheme, float transitionSpeed = 0f);
}
