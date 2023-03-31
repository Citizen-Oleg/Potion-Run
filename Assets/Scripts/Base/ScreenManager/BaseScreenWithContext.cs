namespace Assets.Scripts.Managers.ScreensManager
{
    /// <summary>
    /// Базовый класс для окон с контекстом
    /// Его необходимо использовать, если вы хотите выводить заданную вами
    /// информацию на окно.
    ///
    /// Пример: окно наград, на которое выводится количество монеток,
    /// заработанное пользователем
    /// </summary>
    public abstract class BaseScreenWithContext<TContext> : BaseScreen
        where TContext : BaseContext
    {
        /// <summary>
        /// В перезаписи этого метода вы должны выводить контекст на экран 
        /// </summary>
        /// <param name="context"></param>
        public abstract void ApplyContext(TContext context);
    }
}
