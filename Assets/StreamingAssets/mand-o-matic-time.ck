//---------------|
// mand-o-matic!
// by: Ge Wang (gewang@cs.princeton.edu)
//     Perry R. Cook (prc@cs.princeton.edu)
//------------------|

global float timeScale;
//float timeScale;
//2.0 => timeScale;

// our patch
Mandolin mand => JCRev r => Echo a => Echo b => Echo c => dac;
// set the gain
.95 => r.gain;
// set the reverb mix
.05 => r.mix;
// set max delay for echo
1000::ms => a.max => b.max => c.max;
// set delay for echo
750::ms => a.delay => b.delay => c.delay;
// set the initial effect mix
0.0 => a.mix => b.mix => c.mix;

// scale
[ 0, 2, 4, 7, 9 ] @=> int scale[];

fun void advanceTime(dur duration)
{
    if (timeScale > 0)
    {
        duration / timeScale => now;
    }
    else
    {
        30::ms => now;
    }
}

// shred to modulate the mix
fun void echo_Shred( )
{
    0.0 => float decider => float mix => float old => float inc;

    // time loop
    while( true )
    {
        Math.random2f( 0, 1 ) => decider;
        if( decider < .35 ) 0.0 => mix;
        else if( decider < .55 ) .08 => mix;
        else if( decider < .8 ) .5 => mix;
        else .15 => mix;

        // find the increment
        (mix-old)/1000.0 => inc; 1000 => int n;
        // time loop
        while( n-- )
        {
            // set the mix for a, b, c
            old + inc => old => a.mix => b.mix => c.mix;
            advanceTime(1::ms);
            //1::ms => now;
        }
        // remember the old
        mix => old;
        // let time pass until the next iteration
        advanceTime(Math.random2(2,6)::second);
        //Math.random2(2,6)::second => now;
    }
}

// let echo shred go
spork ~ echo_Shred();

// our main loop
while( true )
{
    // position
    Math.random2f( 0.2, 0.8 ) => mand.pluckPos;
    // frequency...
    scale[Math.random2(0,scale.size()-1)] => int freq;
    220.0 * Math.pow( 1.05946, (Math.random2(0,2)*12) + freq ) => mand.freq;
    // pluck it!
    Math.random2f( 0.2, 0.9 ) => mand.pluck;

    // note: Math.randomf() returns value between 0 and 1
    if( Math.randomf() > 0.9 )
    { advanceTime(500::ms); }
    else if( Math.randomf() > .925 )
    { advanceTime(250::ms); }
    else if( Math.randomf() > .05 )
    { advanceTime(.125::second); }
    else
    {
        1 => int i => int pick_dir;
        // how many times
        4 * Math.random2( 1, 5 ) => int pick;
        0.0 => float pluck;
        0.7 / pick => float inc;
        // time loop
        for( ; i < pick; i++ )
        {
            advanceTime(75::ms);
            //75::ms => now;
            Math.random2f(.2,.3) + i*inc => pluck;
            pluck + -.2 * pick_dir => mand.pluck;
            // simulate pluck direction
            !pick_dir => pick_dir;
        }
        // let time pass for final pluck
        advanceTime(75::ms);
        //75::ms => now;
    }
}
