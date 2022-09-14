using Satchel;
namespace EnemyHPBar;

public class HPBarAnimation {
	public float fps;
	public bool loop;
	public int frame;
	public string name;
}
public class HPBarAnimationController:MonoBehaviour {
	public HPBarAnimation anim;
	public Sprite[] sprites;
	public int currentFrame = 0;
	public bool animating = false;
	private Image img;
	private DateTime lastFrameChange;

	public void Start() {
		img=gameObject.GetAddComponent<Image>();
	}
	public void Init(HPBarAnimation animation) {
		anim = animation;
		// init values
		lastFrameChange = DateTime.MinValue;
		currentFrame = -1;
		animating = true;
		sprites = CustomHPBarAnimation.loadedSprites[anim];
	}
	public void Update() {
		if (!animating) { return; }
		if (lastFrameChange == null || (DateTime.Now - lastFrameChange).TotalMilliseconds > 1000 / anim.fps) {
			currentFrame++;
			if (currentFrame >= sprites.Length) {
				if (anim.loop) {
					currentFrame = 0;
				} else {
					currentFrame = sprites.Length - 1;
				}
			}
			lastFrameChange = DateTime.Now;
		}
		if (img == null) {
			img = gameObject.GetAddComponent<Image>();
		}
		img.sprite = sprites[currentFrame];
	}
}
public static class CustomHPBarAnimation {
	/// <summary>
	/// All currently loaded Sprites
	/// </summary>
	public static Dictionary<HPBarAnimation, Sprite[]> loadedSprites = new Dictionary<HPBarAnimation, Sprite[]>();
	public static HPBarAnimation LoadAnimation(string name, Sprite[] sprites) {
		var anim = new HPBarAnimation {
			name = name,
			fps = 1,
			frame = sprites.Length,
			loop = true
		};
	loadedSprites[anim] = sprites;
		return anim;
	}
}
