using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PixelBattleText
{
	///<summary>A singleton controller for displaying pixel perfect text animations</summary>
	public class PixelBattleTextController : MonoBehaviour
	{
		///<summary>Last instantiated PixelBattleTextController</summary>
		public static PixelBattleTextController singleton;

		///<summary>The parent object of the animated text.
		///Its "pixel grid" will be taken as the base scale for calculating the positioning of each animation</summary>
		public RectTransform canvas;
		
		///<summary>If it is TRUE, This makes the text snap to the canvas "pixel grid".
		///Useful for "chopping" animations that look too smooth if you need a more retro feeling.</summary>
		public bool snapToPixelGrid;
		
		private Shader borderShader;
		private List<TMP_Text[]> letters;
		private Queue<TMP_Text[]> unusedLetters;
		private List<AnimatedTextInstace> animatedTexts = new List<AnimatedTextInstace>();
		private Dictionary<TextAnimation, Material> fontMaterials = new Dictionary<TextAnimation, Material>();
		private GameObject textPrefab;
		private int _texelOffset_id = Shader.PropertyToID("_TexelOffset");

		private TMP_Text[] GetNewText()
		{
			TMP_Text[] text;
			if (unusedLetters.Count == 0)
			{
				var i = Instantiate(textPrefab, canvas, false);
				text = i.GetComponentsInChildren<TMP_Text>();
			}
			else
			{
				text = unusedLetters.Dequeue();
			}
			letters.Add(text);
			return text;
		}

				private Material GetFontMaterial(TextAnimation textAnimation)
		{
#if UNITY_EDITOR
			//if no material is related to this TextAnimation => generate it and realte to this TextAnimation
			if(!fontMaterials.ContainsKey(textAnimation))
				fontMaterials[textAnimation] = new Material(borderShader);
			//Setup material (this is done every time the material is requested for picking up changes made in edit mode)
			var mat = fontMaterials[textAnimation];
			//update material with font texture
			mat.mainTexture = textAnimation.font.atlasTexture;
			//set the texel relative size for keeping a constant 1px line (in font scale) at any text size
			//(this is different to _MainTex_TexelSize as it has to rescale independent from TextSize field in TMPro_Text)
			mat.SetFloat(_texelOffset_id, 1.0f / textAnimation.font.atlasHeight
			* textAnimation.font.faceInfo.lineHeight / (float) textAnimation.textSize);
			return mat;
#else

			if(!fontMaterials.ContainsKey(textAnimation))
			{
				var mat = new Material(borderShader);
				mat.mainTexture = textAnimation.font.atlasTexture;
				mat.SetFloat(_texelOffset_id, 1.0f / textAnimation.font.atlasHeight
				* textAnimation.font.faceInfo.lineHeight / (float) textAnimation.textSize);
				fontMaterials[textAnimation] = mat;
			}
			return fontMaterials[textAnimation];
			
#endif
		}
		
		///<summary>
		///Displays and animates an efimeral text UI element at a given position in canvas world space
		///</summary>
		///<param name="word"> The string to display</param>
		///<param name="textAnimation"> Parameters for animating every letter</param>
		///<param name="position"> Position for where to display the text (canvas space)</param>
		public static void DisplayText(string word, TextAnimation textAnimation, Vector2 position){
#if UNITY_EDITOR
			if(!singleton)
			{
				Debug.LogWarning("There is no PixelBattleController in the scene. Create one in order to use this function.\nThis error only logs in EDITOR MODE, in the project build this will just CRASH");
				return;
			}
#endif
			singleton._DisplayText(word, textAnimation, position);
		}

		private void _DisplayText(string word, TextAnimation textAnimation, Vector2 position)
		{
#if UNITY_EDITOR
			if(!canvas)
			{
				Debug.LogWarning("The 'Canvas' field in the PixelBattleController is unassigned. Assigne it in order to use this function.\nThis error only logs in EDITOR MODE, in the project build this will just CRASH", this);
				return;
			}
#endif

			position.x *= canvas.rect.width;
			position.y *= canvas.rect.height;
			
			Transform[] letterTransforms = new Transform[word.Length];
			TMP_Text[][] wordGraphics = new TMP_Text[word.Length][];
			for (int i = 0; i < word.Length; i++)
			{
				string character = word[i].ToString();
				wordGraphics[i] = GetNewText();

				var alignmentConfig = textAnimation.alignment == TextAnimation.TextAnimationAlignment.Center?
						HorizontalAlignmentOptions.Center
						: textAnimation.alignment == TextAnimation.TextAnimationAlignment.Right?
							HorizontalAlignmentOptions.Right
							: HorizontalAlignmentOptions.Left;

				if(textAnimation.haveBorder){
					wordGraphics[i][0].font = textAnimation.font;
					wordGraphics[i][0].fontMaterial = GetFontMaterial(textAnimation);
					wordGraphics[i][0].gameObject.SetActive(true);
					wordGraphics[i][0].fontSize = textAnimation.textSize;
					wordGraphics[i][0].text = character;
					wordGraphics[i][0].horizontalAlignment = alignmentConfig;
				}
				else
					letters[i][0].gameObject.SetActive(false);

				wordGraphics[i][1].font = textAnimation.font;
				wordGraphics[i][1].fontSize = textAnimation.textSize;
				wordGraphics[i][1].text = character;
				wordGraphics[i][1].horizontalAlignment = alignmentConfig;

				letterTransforms[i] = wordGraphics[i][0].transform.parent;
			}

			var alignmentOffset = textAnimation.alignment == TextAnimation.TextAnimationAlignment.Center?
						-(textAnimation.endSpacing * (word.Length-1))/2.0f
						: textAnimation.alignment == TextAnimation.TextAnimationAlignment.Right?
							-textAnimation.endSpacing * (word.Length - 1)
							: 0;

			var animatedText = new AnimatedTextInstace()
			{
				letterTransforms = letterTransforms,
				letters = wordGraphics,
				props = textAnimation,
				startTime = Time.time,
				pos = (position + new Vector2(alignmentOffset, 0)),
				active = false,
			};

			animatedTexts.Add(animatedText);
		}

		private void RemoveText(int index)
		{
			var text = animatedTexts[index];
			animatedTexts.RemoveAt(index);
			foreach (var letter in text.letters)
			{
				letters.Remove(letter);
				unusedLetters.Enqueue(letter);
				letter[0].transform.parent.gameObject.SetActive(false);
			}
		}

		private void Update()
		{
			for (int i = 0; i < animatedTexts.Count; i++)
			{
				var text = animatedTexts[i];

				if (text.animationFinished)
				{
					RemoveText(i);
					i--;

					continue;
				}

				var props = text.props;
				var letters = text.letters;
				var transforms = text.letterTransforms;
				var start = text.startTime;
				var pos = text.pos;
				var duration = props.transitionDuration;
				var delay = props.perLetterDelay;

				if (!text.active)
				{
					for (int j = 0; j < transforms.Length; j++)
					{
						transforms[j].gameObject.SetActive(true);
					}

					text.active = true;
				}

				//assume all letters have finished their transitions
				//(this will be overwritten in case a letter with an active transition is found while evaluating their progress)
				var allEnded = true;
				
				//setup all letters so they are in the correct position at current frame
				for (int j = 0; j < letters.Length; j++)
				{
					//spacing and position
					var letterStart =   start + delay * (props.invertAnimationOrder ? letters.Length - j : j);
					var letterEnd = letterStart + duration;

					//t is a frame independent progress counter (0-1) one or above means the transition has finished
					var t = Mathf.Clamp01(Mathf.InverseLerp(letterStart, letterEnd, Time.time));

					var pivotT = props.spacingCurve.Evaluate(t);
					var letterPivot = new Vector2(
						Mathf.Lerp(props.initialSpacing * j, props.endSpacing * j, pivotT), 0);

					var additivePosXT = props.offsetCurveX.Evaluate(t);
					var additivePosYT = props.offsetCurveY.Evaluate(t);

					var additiveX = Mathf.Lerp(props.initialOffset.x, props.endOffset.x, additivePosXT);
					var additiveY = Mathf.Lerp(props.initialOffset.y, props.endOffset.y, additivePosYT);

					var letterAdditivePos = new Vector2(additiveX, additiveY);

					Vector2 pixelPosition = (pos + letterPivot + letterAdditivePos);
					
					transforms[j].GetComponent<RectTransform>().anchoredPosition = snapToPixelGrid? new Vector2( Mathf.Floor(pixelPosition.x), Mathf.Floor(pixelPosition.y)) : pixelPosition;

					letters[j][1].color = props.fillColorInTime.Evaluate(t);
					
					//border color
					if(props.haveBorder)
						letters[j][0].color = props.borderColorInTime.Evaluate(t);

					if (t < 1)
						allEnded = false;
				}

				if (allEnded)
				{
					text.animationFinished = true;
					text.startTime = Time.time;
					animatedTexts[i] = text;
				}
			}
		}

		private void Awake()
		{
			if (singleton)
				Destroy(this);
			else
				singleton = this;
		}
		
		// Start is called before the first frame update
		private  void Start()
		{
			letters = new List<TMP_Text[]>();
			unusedLetters = new Queue<TMP_Text[]>();
			animatedTexts = new List<AnimatedTextInstace>();
			textPrefab = Resources.Load("pixel_text") as GameObject;
			borderShader = (Shader)Resources.Load("PixelBorder");
			
			//set text prefab attached to the right lower corner of the canvas to simplify positioning calculations
			var textPrefabTransform = textPrefab.GetComponent<RectTransform>();
			textPrefabTransform.anchorMax = Vector2.zero;
			textPrefabTransform.anchorMin = Vector2.zero;
		}

		private void Destroy(){
			//destroy all created materials
			foreach (var item in fontMaterials)
				Destroy(item.Value);
			
			Resources.UnloadAsset(textPrefab);
			Resources.UnloadAsset(borderShader);
		}
	}
}