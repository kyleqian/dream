global float timeScale;
//1.0 => float timeScale;
5::ms => dur atomicDuration;

fun void advanceTimeScaled(dur originalDuration)
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

SinOsc foo => dac;
while( true )
{
    advanceTimeScaled(100::ms);
    Math.random2f(300, 1000) => foo.freq;
}
