using Ion;
using Ion.Core;
using Ion.Input;
using Ion.Text;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Windows;
using System.Windows.Input;

namespace Ion.Tools.Random;

public record class AppViewModel() : AppToolViewModel()
{
    /*
    The lowercase letter l (ell) and the digit 1 (one)
    The uppercase letter I (capital i) and the digit 1 (one)
    The uppercase letter O (capital o) and the digit 0 (zero)
    The lowercase letter o and the digit 0 (zero)
    The letter S and the digit 5
    The letter B and the digit 8
    The letter Z and the digit 2
    The letter C and the opening round bracket (
    */
    public Boolean Ambiguous { get => Get(false); set => Set(value); }

    public static List<char> AmbiguousCharacters = new List<char>
    {
        'l', 'I', '1', 'o', 'O', '0', 'S', '5', 'B', '8', 'Z', '2', 'C', '(', 'j', ';',
    };

    public Boolean Lower { get => Get(true); set => Set(value); }

    public Boolean Numbers { get => Get(true); set => Set(value); }

    public Boolean Special { get => Get(true); set => Set(value); }

    public Boolean Upper { get => Get(true); set => Set(value); }

    public String LengthMaximum { get => Get("16"); set => Set(value); }

    public String LengthMinimum { get => Get("16"); set => Set(value); }

    public String MinimumNumbers { get => Get("0"); set => Set(value); }

    public String MinimumSpecial { get => Get("0"); set => Set(value); }

    public String Text { get => Get(""); set => Set(value); }

    public override string Title => nameof(Random);

    private static readonly System.Random random = new System.Random();

    public string getRandomString(int minLength, int maxLength, int minNumbers, int minSpecial)
    {
        /// Validate input
        if (minLength < 0 || maxLength < minLength)
            throw new ArgumentException("Invalid length range.");
        if (minNumbers < 0 || minSpecial < 0)
            throw new ArgumentException("Minimum counts cannot be negative.");

        /// Ensure minimum requirements are feasible
        if (minNumbers + minSpecial > maxLength)
            throw new ArgumentException("Minimum number and special character requirements exceed maximum length.");

        /// Determine actual length of string
        int length = random.Next(minLength, maxLength + 1);

        /// Ensure enough space for minimum required characters
        int lengthRemaining = length - minNumbers - minSpecial;
        if (lengthRemaining < 0)
            throw new ArgumentException("Not enough length to satisfy minimum requirements.");

        /// Build character pool
        var all = "";
        var numbers = "";
        var special = "";

        if (Lower)
        {
            all += Characters.Lower;
        }

        if (Upper)
        {
            all += Characters.Upper;
        }

        var result = new List<char>();

        /// Add minimum numbers
        if (Numbers)
        {
            all += Characters.Numbers;
            numbers = Characters.Numbers;

            /// Remove ambiguous numbers
            if (!Ambiguous)
            {
                numbers = new string([.. numbers.Where(x => !AmbiguousCharacters.Contains(x))]);
            }

            if (numbers.Length > 0)
            {
                for (int i = 0; i < minNumbers; i++)
                {
                    result.Add(numbers[random.Next(numbers.Length)]);
                }
            }
        }

        /// Add minimum special
        if (Special)
        {
            all += Characters.Special;
            special = Characters.Special;

            /// Remove ambiguous special
            if (!Ambiguous)
            {
                special = new string([.. special.Where(x => !AmbiguousCharacters.Contains(x))]);
            }

            if (special.Length > 0)
            {
                for (int i = 0; i < minSpecial; i++)
                {
                    result.Add(special[random.Next(special.Length)]);
                }
            }
        }
        /// Remove ambiguous
        if (!Ambiguous)
        {
            all = new string([.. all.Where(c => !AmbiguousCharacters.Contains(c))]);
        }

        /// Add rest of any
        if (all.Length > 0)
        {
            for (int i = 0; i < lengthRemaining; i++)
            {
                result.Add(all[random.Next(all.Length)]);
            }
        }

        if (result.Count > 0)
        {
            /// Shuffle result to avoid predictable patterns
            for (int i = 0; i < result.Count; i++)
            {
                int j = random.Next(result.Count);
                (result[i], result[j]) = (result[j], result[i]);
            }

            return new string([.. result]);
        }

        return null;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "<Pending>")]
    public void Do()
    {
        int.TryParse(LengthMinimum, out int lengthMinimum);
        int.TryParse(LengthMaximum, out int lengthMaximum);

        int.TryParse(MinimumNumbers, out int minimumNumbers);
        int.TryParse(MinimumSpecial, out int minimumSpecial);

        Text = getRandomString(lengthMinimum, lengthMaximum, minimumNumbers, minimumSpecial);
    }

    public ICommand CopyCommand => Commands[nameof(CopyCommand)] ??= new RelayCommand(() =>
    {
        Clipboard.SetText(Text);
        ///new ToastContentBuilder().AddText("Copied!").Show();
    }, () => true);

    public ICommand DoCommand => Commands[nameof(DoCommand)] ??= new RelayCommand(() => Do(), () => true);
}
