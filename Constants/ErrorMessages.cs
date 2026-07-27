namespace WebApplication2.Constants
{
    public static class ErrorMessages
    {
        public const string NotFound =
            "{0} not found.";

        public const string Required =
            "{0} is required.";

        public const string AssignedEntityNotFound =
            "The assigned {0} does not exist.";

        public const string ActiveDuplicate =
            "An active {0} with this {1} already exists{2}.";

        public const string AnotherActiveDuplicate =
            "Another active {0} with this {1} already exists{2}.";

        public const string CannotDeleteWithActiveChildren =
            "The {0} cannot be deleted because it contains active {1}.";

        public const string CannotDeleteBecauseAssociated =
            "The {0} cannot be deleted because it is associated with a {1}.";
    }
}
