class Pattern {
	[0, 4, 7] @=> int major[];
	[0, 3, 7] @=> int minor[];

	[0, 7, 12, 16] @=> int majorArp[];
	[0, 7, 12, 15] @=> int minorArp[];
}

class BPM {
	static dur N2, N4, N8, N16, N32;
	float bpm;

	public void setTempo(float tempo) {
		(60.0 / tempo)::second => N4;
		tempo => bpm;
		2 * N4 => N2;
		0.5 * N4 => N8;
		0.5 * N8 => N16;
		0.5 * N16 => N32;
	}
}

class Block {
	dur size;
	public void setSize(dur T) {
		T => size;
	}
}

BeeThree b3 => ADSR adsr => LPF lp => NRev rev => dac;
BeeThree b32 => ADSR adsr2 => lp;
0.4 => b3.gain;
0.4 => b32.gain;

//global float timeScale;
1 => float timeScale;
5::ms => dur atomicDuration;

fun void advanceScaledTime(dur originalDuration)
{
    now => time startTime;
    time endTime;
    do {
        if (timeScale == 0)
        {
            // Guaranteed to not exit the loop.
            now + 2 * atomicDuration => endTime;
        }
        else
        {
            startTime + originalDuration / timeScale => endTime;
        }
        
        atomicDuration => now;
    } while (now < endTime);
}

// Parameter settings.
1800 => lp.freq;
0.2 => rev.mix;
adsr.set(30::ms, 0::ms, 1.0, 50::ms);

BPM bpm;
bpm.setTempo(120.0);

Block block;
block.setSize(2 * bpm.N2);

Pattern patternSet;

// Synchronize time with other blocks.
block.size - (now % block.size) => now;

while (true) {
    playPattern(62, 0);
    playPattern(57, 0);
    playPattern(59, 1);
    playPattern(54, 1);
    playPattern(55, 0);
    playPattern(50, 0);
    playPattern(55, 0);
    playPattern(57, 0);
}

fun void playPattern(int root, int patternNum) {
    spork ~ playBass(root);
    
}

fun void playBass(int root) {
    Std.mtof(root - 24) => b32.freq;
    1.0 => b32.noteOn;
    adsr2.keyOn();
    advanceScaledTime(block.size);
    adsr2.keyOff();
    advanceScaledTime(100::ms);
}
