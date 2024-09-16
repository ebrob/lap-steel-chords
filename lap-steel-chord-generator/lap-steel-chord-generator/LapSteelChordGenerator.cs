using System.Text;
using Bach.Model;
using Bach.Model.Instruments;

public class LapSteelChordGenerator
{
    public async Task Generate(string targetDir, int countFrets, string tuningName)
    {
        var instrument = StringedInstrument.Create( "guitar", countFrets, tuningName );
        var roots = new List<PitchClass>
        {
            PitchClass.C,
            PitchClass.CSharp,
            PitchClass.D,
            PitchClass.DSharp,
            PitchClass.E,
            PitchClass.F,
            PitchClass.FSharp,
            PitchClass.G,
            PitchClass.GSharp,
            PitchClass.A,
            PitchClass.ASharp,
            PitchClass.B
        };
        var formulas = new List<ChordFormulaModel>
        {
            new("Major"),
            new("Major7"),
            new("Major6"),
            new("Augmented"),
            new("Augmented7"),
            new("AugmentedMajor7"),
            new("Diminished"),
            new("Diminished7"),
            new("Minor"),
            new("Minor7"),
            new("Minor6"),
        };

        var js = new StringBuilder();

        foreach( var root in roots )
        {
            foreach( var formula in formulas )
            {
                var chord = new Chord( root, formula.Formula );
                var varName = $"chord{root.ToString().Replace( "#", "Sharp" )}Root{formula.Formula}";
                //var title = $"{root.ToString()} {formula.Title}";
                var oneChordJs = GenerateChord( instrument, chord, varName );
                js.AppendLine( oneChordJs );
            }
        }

        var fileName = $"{tuningName}-{instrument.PositionCount}frets.js";
        var targetPath = Path.Combine( targetDir, fileName );

        await File.WriteAllTextAsync( targetPath, js.ToString() );
    }

    private static string GenerateChord(StringedInstrument instrument, Chord chord, string varName )
    {
        var js = new StringBuilder();

        var adjustedTuning = instrument.Tuning.Pitches.Reverse().ToList();
        var tuning = string.Join( ",", adjustedTuning.Select( x => $"'{x.PitchClass}'" ) );

        js.AppendLine( $$"""
                         var {{varName}} = {
                               settings: {
                               color: '#000000',
                               strings: {{adjustedTuning.Count}},
                               frets: {{instrument.PositionCount}},
                               position: 1,
                               nutSize: 0.65,
                               fingerSize: 0.75,
                               strokeWidth: 2,
                               style: 'normal',
                               orientation: 'horizontal',
                               tuning: [ {{tuning}} ]
                               },
                         """ );

        js.AppendLine(
            @"
      chord:  {
      fingers: ["
        );
        var fingers = GetFingers( instrument.PositionCount, adjustedTuning, chord );

        js.AppendLine( fingers );

        js.AppendLine(
            @"],
      barres: [],
      }
    };"
        );

        return js.ToString();
    }

    private static string GetFingers(
        int countFrets,
        IReadOnlyList<Pitch> stringPitches,
        Chord chord)
    {
        var js = new StringBuilder();
        var semitone = new Interval( IntervalQuantity.Second,IntervalQuality.Minor );
        for( var stringIndex = 0; stringIndex < stringPitches.Count; stringIndex++ )
        {
            var fretPitch = stringPitches[stringIndex];
            for( var fretIndex = 0; fretIndex < countFrets; fretIndex++ )
            {
                fretPitch += semitone;
                foreach( var pitch in chord.PitchClasses )
                {
                    if( fretPitch.PitchClass != pitch )
                        continue;

                    // finger at string 2, fret 3, with text '3', colored red and has class '.red'
                    //[2, 3, { text: '3', color: '#F00', className: 'red' }],
                    //var finger = $"[{stringIndex + 1}, {fretIndex+1}, {{ text: '{pitch}', color: '#F00', className: 'red' }}],";
                    var finger = $"[{stringIndex + 1}, {fretIndex+1}, {{ text: '{pitch}' }}],";
                    js.AppendLine( finger );
                }
            }
        }

        return js.ToString();
    }

    private class ChordFormulaModel
    {
        public string Formula { get; }
        public string Title { get; }

        public ChordFormulaModel(string formula)
        {
            Formula = formula;
            Title = formula;
        }
        public ChordFormulaModel(string formula, string title)
        {
            Formula = formula;
            Title = title;
        }
    }

}