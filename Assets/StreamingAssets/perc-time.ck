Shakers shakeL => JCRev r => dac;
Shakers shakeR => r => dac;
.025 => r.mix;

global float timeScaleL;
global float timeScaleR;
//1 => float timeScaleL;
//1 => float timeScaleR;
5::ms => dur atomicDuration;

fun void advanceScaledTime(dur originalDuration, int R)
{
    now => time startTime;
    0::ms => dur timePaused;
    float timeScale;
    time endTime;
    do {
        if (R == 0) {
            timeScaleL => timeScale;
        } else {
            timeScaleR => timeScale;
        }
        
        if (timeScale == 0) {            
            // Guaranteed to not exit the loop.
            atomicDuration +=> timePaused;
            now + 2 * atomicDuration => endTime;
        } else {            
            startTime + timePaused + originalDuration / timeScale => endTime;
        }
        
        atomicDuration => now;
    } while (now < endTime);
}

spork ~ playL();
spork ~ playR();

while (true) {
    1::day => now;
}

fun void playL() {
    0 => shakeL.which;
    Std.mtof( Math.random2f( 0.0, 128.0 ) ) => shakeL.freq;
    Math.random2f( 0, 128 ) => shakeL.objects;
    while (true) {
        Math.random2f( 0.8, 1.3 ) => shakeL.noteOn;
        advanceScaledTime(1::second, 0);
    }
}

fun void playR() {
    3 => shakeR.which;
    Std.mtof( Math.random2f( 0.0, 128.0 ) ) => shakeR.freq;
    Math.random2f( 0, 128 ) => shakeR.objects;
    while (true) {
        Math.random2f( 0.8, 1.3 ) => shakeR.noteOn;
        advanceScaledTime(1::second, 1);
    }
}
