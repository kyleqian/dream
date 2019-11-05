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
BeeThree b4 => ADSR adsr3 => lp;
0.2 => b3.gain;
0.2 => b32.gain;
0.4 => b4.gain;

global float timeScaleL;
global float timeScaleR;
//1 => float timeScaleL;
//1 => float timeScaleR;
5::ms => dur atomicDuration;
1 => int connectedL;
1 => int connectedR;

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
            if (R == 0 && connectedL == 1) {
                b3 =< adsr;
                b32 =< adsr2;
                0 => connectedL;
            } else if (R == 1 && connectedR == 1) {
                b4 =< adsr3;
                0 => connectedR;
            }
            
            // Guaranteed to not exit the loop.
            atomicDuration +=> timePaused;
            now + 2 * atomicDuration => endTime;
        } else {
            if (R == 0 && connectedL == 0) {
                b3 => adsr;
                b32 => adsr2;
                1 => connectedL;
            } else if (R == 1 && connectedR == 0) {
                b4 => adsr3;
                1 => connectedR;
            }
            
            startTime + timePaused + originalDuration / timeScale => endTime;
        }
        
        atomicDuration => now;
    } while (now < endTime);
}

// Parameter settings.
1800 => lp.freq;
0.2 => rev.mix;
adsr.set(30::ms, 0::ms, 1.0, 50::ms);
adsr2.set(50::ms, 0::ms, 1.0, 100::ms);
adsr3.set(30::ms, 0::ms, 1.0, 50::ms);

BPM bpm;
bpm.setTempo(120.0);

Block block;
block.setSize(2 * bpm.N2);

Pattern patternSet;

// Synchronize time with other blocks.
block.size - (now % block.size) => now;

spork ~ loopMelody();
spork ~ loopPattern();

while (true) {
    1::day => now;
}

fun void loopMelody() {
    while (true) {
        playMelody();
    }
}

fun void loopPattern() {
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
}

fun void playPattern(int root, int patternNum) {
    //spork ~ playBass(root);
    
	if (patternNum == 0) {
		for (0 => int i; i < patternSet.majorArp.cap(); i++) {
			root + patternSet.majorArp[i] => int thisNote;
			Std.mtof(thisNote) => b3.freq;
			1.0 => b3.noteOn;
            adsr.keyOn();
			advanceScaledTime(bpm.N4, 0);
            adsr.keyOff();
            advanceScaledTime(50::ms, 0);
		}
	} else if (patternNum == 1) {
		for (0 => int i; i < patternSet.minorArp.cap(); i++) {
			root + patternSet.minorArp[i] => int thisNote;
			Std.mtof(thisNote) => b3.freq;
			1.0 => b3.noteOn;
            adsr.keyOn();
			advanceScaledTime(bpm.N4, 0);
            adsr.keyOff();
            advanceScaledTime(50::ms, 0);
		}
	}
}

fun void playBass(int root) {
    Std.mtof(root - 24) => b32.freq;
    1.0 => b32.noteOn;
    adsr2.keyOn();
    advanceScaledTime(block.size, 0);
    adsr2.keyOff();
    advanceScaledTime(100::ms, 0);
}

fun void playMelody() {
    [78, 76, 74, 73, 71, 69, 71, 73] @=> int melody[];
    for (0 => int i; i < melody.cap(); i++) {
        Std.mtof(melody[i]) => b4.freq;
        1.0 => b4.noteOn;
        adsr2.keyOn();
        advanceScaledTime((bpm.N4 + 50::ms) * 4, 1);
        adsr2.keyOff();
    }
}
