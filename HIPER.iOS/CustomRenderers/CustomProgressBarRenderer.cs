using CoreGraphics;
using HIPER.iOS.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ProgressBar), typeof(CustomProgressBarRenderer))]
[assembly: ExportRenderer(typeof(RadioButton), typeof(CustomRadioButtonRenderer))]
namespace HIPER.iOS.CustomRenderers
{
    public class CustomProgressBarRenderer : ProgressBarRenderer
    {
        public CustomProgressBarRenderer()
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<ProgressBar> e)
        {
            base.OnElementChanged(e);

            LayoutSubviews();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            float x = 1.0f;
            float y = 3.0f;

            CGAffineTransform transform = CGAffineTransform.MakeScale(x, y);
            Transform = transform;
        }

    }



    public class CustomRadioButtonRenderer : RadioButtonRenderer
    {
        public CustomRadioButtonRenderer()
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<RadioButton> e)
        {
            base.OnElementChanged(e);
            LayoutSubviews();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            float x = 1.2f;
            float y = 1.2f;

            CGAffineTransform transform = CGAffineTransform.MakeScale(x, y);
            Transform = transform;
        }
    }
}
