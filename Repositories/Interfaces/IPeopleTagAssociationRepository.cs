using WebApplication2.Models;

namespace WebApplication2.Repositories.Interfaces
{
    public interface IPeopleTagAssociationRepository
    {
        Task<PeopleTagAssociation?> GetByPersonIdAsync(
            int personId);

        Task AddAsync(
            PeopleTagAssociation association);

        void Remove(
            PeopleTagAssociation association);

        Task SaveChangesAsync();
    }
}