using UnityEngine;
using TMPro;


namespace PixelBattleText
{
	///<summary>The parameters of a text animation that can be fed to a PixelBattleTextController in order to display animated text</summary>
	[CreateAssetMenu(fileName = "newTextAnimation", menuName = "PixelBattleText/TextAnimation")]
	public class TextAnimation: ScriptableObject
	{
		public enum TextAnimationAlignment
		{
			Left,
			Center,
			Right
		}

		[Header("Font Controls")]
		///<summary>The font to use for rendering the text animation.</summary>
		public TMP_FontAsset font;
		///<summary>The size of the text element to display.</summary>
		public int textSize = 16;
		///<summary>The text alignment for the spacing. Also modifies the letter "pivot" positioning.</summary>
		public TextAnimationAlignment alignment;

		[Header("Time Controls")]
		///<summary>The duration of the animation of one single letter.</summary>
		public float transitionDuration = .5f;
		///<summary>The delay of each letter.
		///A letter can only start to animate once all previous letters have ended their delays and its own.</summary>
		public float perLetterDelay = 0.05f;
		///<summary>When TRUE, each letter start animating in the inverse order.
		///(The animation direction isn't affected by this, only the order is affected)</summary>
		public bool invertAnimationOrder = false;
		
		[Header("Spacing Animation")]
		///<summary>Determines the initial space (in canvas pixels) between the pivots of the letters.
		///This space is taken in the animation as the starting point for the position of the letters.</summary>
		public float initialSpacing = 9;
		///<summary>Determines the final space (in canvas pixels) between the pivots of the letters.
		///This space is taken in the animation as the end point for the position of the letters.</summary>
		public float endSpacing = 9;
		///<summary>Determines the progress of the animation from initial spacing to end spacing following a curve from 0 to 1, where 0 is the initial spacing and 1 the end.</summary>
		public AnimationCurve spacingCurve = AnimationCurve.Constant(0, 1, 1);

		[Header("Offset Animation")]
		///<summary>Determines a vector (in canvas pixels) to be added to the pivot at the begining of the animation.</summary>
		public Vector2 initialOffset = Vector2.zero;
		///<summary>Determines a vector (in canvas pixels) to be added to the pivot at the end of the animation.</summary>
		public Vector2 endOffset = Vector2.zero;
		///<summary>Determines the progress of the animation from Initial Offset to End Offset following a curve from 0 to 1, where 0 is the initial spacing and 1 the end.
		///This only affects the X factor</summary>
		public AnimationCurve offsetCurveX = AnimationCurve.Constant(0, 1, 1);
		///<summary>Determines the progress of the animation from Initial Offset to End Offset following a curve from 0 to 1, where 0 is the initial spacing and 1 the end.
		///This only affects the X factor</summary>
		public AnimationCurve offsetCurveY = AnimationCurve.Constant(0, 1, 1);

		[Header("Color Animation")]
		///<summary>Determines a color gradient that represents the changing of the letter´s inner color during the animation.</summary>
		public Gradient fillColorInTime;
		///<summary>Determines a color gradient that represents the changing of the letter´s border color during the animation.</summary>
		public bool haveBorder;
		///<summary>Determines whether the edge of the letters should be animated or not even displayed.</summary>
		public Gradient borderColorInTime;
	}
}