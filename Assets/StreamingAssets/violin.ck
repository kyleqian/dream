Bowed bow => dac;

global int bowOn;
global int note;
//1 => int bowOn;
//1 => int note;

5::ms => dur atomicDuration;
[55, 62, 69, 76] @=> int openStrings[];

while (true) {
    0.5 => bow.bowPressure;
    1.0 => bow.bowPosition;
    0 => bow.vibratoFreq;
    0 => bow.vibratoGain;
    1 => bow.volume;

    //<<< "---", "" >>>;
    //<<< "bow pressure:", bow.bowPressure() >>>;
    //<<< "bow position:", bow.bowPosition() >>>;
    //<<< "vibrato freq:", bow.vibratoFreq() >>>;
    //<<< "vibrato gain:", bow.vibratoGain() >>>;
    //<<< "volume:", bow.volume() >>>;
    
    if (bowOn == 1) {
        0.8 => bow.noteOn;    
    } else {
        0.2 => bow.noteOff;
    }
    
    openStrings[note] => Std.mtof => bow.freq;

    atomicDuration => now;
}
