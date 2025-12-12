using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  CODE SERVICE
// =====================================
public class CodeItemTranslation : Entity<Guid>
{
    public Guid CodeItemId { get; private set; }
    public Guid LanguageId { get; private set; }
    public string Name { get; private set; } = null!;

    public virtual CodeItem CodeItem { get; private set; } = null!;
    public virtual Language Language { get; private set; } = null!;
}
