////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2009, Daniel Kollmann
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list of conditions
//   and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list of
//   conditions and the following disclaimer in the documentation and/or other materials provided
//   with the distribution.
//
// - Neither the name of Daniel Kollmann nor the names of its contributors may be used to endorse
//   or promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY
// WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace TTRider.UI
{
    public struct HSBColor
    {
        float hue;
        float saturation;
        float brightness;
        int alpha;

        public HSBColor(float h, float s, float b)
        {
            this.alpha = 0xff;
            this.hue = Math.Min(Math.Max(h, 0), 255);
            this.saturation = Math.Min(Math.Max(s, 0), 255);
            this.brightness = Math.Min(Math.Max(b, 0), 255);
        }

        public HSBColor(int a, float h, float s, float b)
        {
            this.alpha = a;
            this.hue = Math.Min(Math.Max(h, 0), 255);
            this.saturation = Math.Min(Math.Max(s, 0), 255);
            this.brightness = Math.Min(Math.Max(b, 0), 255);
        }

        public HSBColor(Color color)
        {
            HSBColor temp = FromColor(color);
            this.alpha = temp.alpha;
            this.hue = temp.hue;
            this.saturation = temp.saturation;
            this.brightness = temp.brightness;
        }

        public float H
        {
            get
            {
                return hue;
            }
        }

        public float S
        {
            get
            {
                return saturation;
            }
        }

        public float B
        {
            get
            {
                return brightness;
            }
        }

        public int A
        {
            get
            {
                return alpha;
            }
        }

        public Color Color
        {
            get
            {
                return FromHSB(this);
            }
        }

        public static Color ShiftHue(Color c, float hueDelta)
        {
            HSBColor hsb = HSBColor.FromColor(c);
            hsb.hue += hueDelta;
            hsb.hue = Math.Min(Math.Max(hsb.hue, 0), 255);
            return FromHSB(hsb);
        }

        public static Color ShiftSaturation(Color c, float saturationDelta)
        {
            HSBColor hsb = HSBColor.FromColor(c);
            hsb.saturation += saturationDelta;
            hsb.saturation = Math.Min(Math.Max(hsb.saturation, 0), 255);
            return FromHSB(hsb);
        }

        public static Color ShiftBrighness(Color c, float brightnessDelta)
        {
            HSBColor hsb = HSBColor.FromColor(c);
            hsb.brightness += brightnessDelta;
            hsb.brightness = Math.Min(Math.Max(hsb.brightness, 0), 255);
            return FromHSB(hsb);
        }

        public static Color FromHSB(HSBColor hsbColor)
        {
            float red = hsbColor.brightness;
            float green = hsbColor.brightness;
            float blue = hsbColor.brightness;

            if (hsbColor.saturation != 0)
            {
                float max = hsbColor.brightness;
                float dif = hsbColor.brightness * hsbColor.saturation / 255f;
                float min = hsbColor.brightness - dif;

                float h = hsbColor.hue * 360f / 255f;

                if (h < 60f)
                {
                    red = max;
                    green = h * dif / 60f + min;
                    blue = min;
                }
                else if (h < 120f)
                {
                    red = -(h - 120f) * dif / 60f + min;
                    green = max;
                    blue = min;
                }
                else if (h < 180f)
                {
                    red = min;
                    green = max;
                    blue = (h - 120f) * dif / 60f + min;
                }
                else if (h < 240f)
                {
                    red = min;
                    green = -(h - 240f) * dif / 60f + min;
                    blue = max;
                }
                else if (h < 300f)
                {
                    red = (h - 240f) * dif / 60f + min;
                    green = min;
                    blue = max;
                }
                else if (h <= 360f)
                {
                    red = max;
                    green = min;
                    blue = -(h - 360f) * dif / 60 + min;
                }
                else
                {
                    red = 0;
                    green = 0;
                    blue = 0;
                }
            }

            return Color.FromArgb(hsbColor.alpha, (int)Math.Round(Math.Min(Math.Max(red, 0), 255)), (int)Math.Round(Math.Min(Math.Max(green, 0), 255)),
                                  (int)Math.Round(Math.Min(Math.Max(blue, 0), 255)));
        }

        public static HSBColor FromColor(Color color)
        {
            HSBColor ret = new HSBColor(0f, 0f, 0f);
            ret.alpha = color.A;

            float red = color.R;
            float green = color.G;
            float blue = color.B;

            float max = Math.Max(red, Math.Max(green, blue));

            if (max <= 0)
            {
                return ret;
            }

            float min = Math.Min(red, Math.Min(green, blue));
            float dif = max - min;

            if (max > min)
            {
                if (green == max)
                {
                    ret.hue = (blue - red) / dif * 60f + 120f;
                }
                else if (blue == max)
                {
                    ret.hue = (red - green) / dif * 60f + 240f;
                }
                else if (blue > green)
                {
                    ret.hue = (green - blue) / dif * 60f + 360f;
                }
                else
                {
                    ret.hue = (green - blue) / dif * 60f;
                }

                if (ret.hue < 0)
                {
                    ret.hue = ret.hue + 360f;
                }
            }
            else
            {
                ret.hue = 0;
            }

            ret.hue *= 255f / 360f;
            ret.saturation = (dif / max) * 255f;
            ret.brightness = max;

            return ret;
        }
    }
}
