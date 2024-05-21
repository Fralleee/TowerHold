public class StyleManager : Singleton<StyleManager>
{
	public StyleSettings StyleSettings;

	public static StyleSettings Styles => Instance.StyleSettings;

	protected override void Awake()
	{
		base.Awake();
		DontDestroyOnLoad(gameObject);
	}
}
