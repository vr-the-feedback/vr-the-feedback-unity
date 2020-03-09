namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter9
{
    public class Page8_0 : IStaticCodeBook
    {
        public int Dimensions { get; } = 2;

        public byte[] LengthList { get; } = {
         1, 4, 4, 7, 7, 8, 8, 8, 8, 9, 9,10, 9,11,10, 4,
         6, 6, 8, 8, 9, 9, 9, 9,10,10,11,10,12,10, 4, 6,
         6, 8, 8, 9,10, 9, 9,10,10,11,11,12,12, 7, 8, 8,
        10,10,11,11,10,10,11,11,12,12,13,12, 7, 8, 8,10,
        10,11,11,10,10,11,11,12,12,12,13, 8,10, 9,11,11,
        12,12,11,11,12,12,13,13,14,13, 8, 9, 9,11,11,12,
        12,11,12,12,12,13,13,14,13, 8, 9, 9,10,10,12,11,
        13,12,13,13,14,13,15,14, 8, 9, 9,10,10,11,12,12,
        12,13,13,13,14,14,14, 9,10,10,12,11,13,12,13,13,
        14,13,14,14,14,15, 9,10,10,11,12,12,12,13,13,14,
        14,14,15,15,15,10,11,11,12,12,13,13,14,14,14,14,
        15,14,16,15,10,11,11,12,12,13,13,13,14,14,14,14,
        14,15,16,11,12,12,13,13,14,13,14,14,15,14,15,16,
        16,16,11,12,12,13,13,14,13,14,14,15,15,15,16,15,
        15,
};

        public CodeBookMapType MapType { get; } = (CodeBookMapType)1;
        public int QuantMin { get; } = -520986624;
        public int QuantDelta { get; } = 1620377600;
        public int Quant { get; } = 4;
        public int QuantSequenceP { get; } = 0;

        public int[] QuantList { get; } = {
        7,
        6,
        8,
        5,
        9,
        4,
        10,
        3,
        11,
        2,
        12,
        1,
        13,
        0,
        14,
};
    }
}