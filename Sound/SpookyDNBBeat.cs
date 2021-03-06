/*
The MIT License

Copyright (c) 2015 Martin Pichlmair

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

/*
Original source by Jakob Schmid: http://www.schmid.dk/talks/2015-01-29-spilbar/Schmid-2015-01-29-Spilbar-Way_Beyond.pdf
*/

using UnityEngine;

class SpookyDNBBeat : MonoBehaviour
{
    float s = 0;

    void OnAudioFilterRead(float[] data, int channels)
    {
        int smp = 0, length = data.Length;

        // int sampleRate = AudioSettings.outputSampleRate;

        while (smp < length)
        {
            s = ++s % 288000;
            float p = (s / 288000) * 0.5f;
            float pBar = (p * 8) % 1;
            float hhAmp = (0.13f + ((pBar * 4) % 1) * -0.09f);

            // mixer
            float output = BD(pBar * 8 / 3) * 0.8f
                + HH((pBar * 8) % 1) * hhAmp
                + bass(p) * 0.2f + bass(p - 0.024f) * 0.1f;

            for (int c = 0; c < channels; ++c)
                data[smp++] = output;
        }
    }

    // Bassdrum: sine with pitch and amplitude envelope
    float BD(float p)
    {
        float env = Mathf.Clamp01(0.1f - (p % 1f)) * 10f;
        float fr = 30f + env * 100f;
        float ph = (p % 1f) * fr;
        return Mathf.Sin((ph % 1f) * 6.28f) * env;
    }

    // Hihat: noise with amplitude envelope
    float HH(float p)
    {
        return Mathf.PerlinNoise(p * 2000, 0f) * (1f - p);
    }

    // Spooky bass: FM synth
    float bass(float p)
    {
        return Mathf.Sin(p * 4000 + Mathf.Sin( p * 4000
             + Mathf.Sin(p * 3.28f) * 1111))
             * Mathf.Sin(((p * 64 / 3f) % 1) * 3.141f);
    }
}
