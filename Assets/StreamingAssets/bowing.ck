global float bowIntensity;
global float thePitch;
//global Event bowChangedDir;

// sound
Moog sqr => Gain product => NRev r => dac;
// envelope generator
Step step => OnePole smooth => product;
// pitch smooth
Step pitchStep => OnePole pitchSmooth => blackhole;
// multiply
3 => product.op;
// set the smoothing
.995 => smooth.pole;
.995 => pitchSmooth.pole;

// set reverb mix
.1 => r.mix;
// set default frequency
110 => sqr.freq;
// note one
1 => sqr.noteOn;

<<< "running chuck code..." >>>;

// spork update shred
spork ~ update( 1::ms );

while( true )
{
    1::second => now;
}

fun void update( dur T )
{
    while( true )
    {
        // set the next value
        Math.pow(bowIntensity, 2)*5=> step.next;
        // update the pitch
        thePitch => pitchStep.next;
        // set the pitch
        pitchSmooth.last() => Std.mtof => sqr.freq;
        // time step
        T => now;
    }
}