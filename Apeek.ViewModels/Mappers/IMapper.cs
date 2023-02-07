namespace Apeek.ViewModels.Mappers
{
    public interface IMapper<Src,Dest>
    {
        Dest Map(Src source, Dest dest);
    }
}