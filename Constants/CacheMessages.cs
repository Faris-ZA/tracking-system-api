namespace WebApplication2.Constants
{
    public static class CacheMessages
    {
        public const string PersonCreateFailed =
            "Person was created in database, but cache update failed.";

        public const string PersonUpdateFailed =
            "Person was updated in database, but cache update failed.";

        public const string PersonDeleteFailed =
            "Person was deleted in database, but cache removal failed.";

        public const string PeopleCacheIncomplete =
            "People cache is missing or incomplete. Falling back to database.";

        public const string PeopleCacheFailed =
            "People cache failed. Falling back to database.";

        public const string TagCreateFailed =
            "Tag was created in database, but cache update failed.";

        public const string TagUpdateFailed =
            "Tag was updated in database, but cache update failed.";

        public const string TagDeleteFailed =
            "Tag was deleted in database, but cache removal failed.";

        public const string TagCacheIncomplete =
            "Tag cache is missing or incomplete. Falling back to database.";

        public const string TagCacheFailed =
            "Tag cache failed. Falling back to database.";

        public const string AssociationCreateCacheFailed =
            "Association was created in database, but cache synchronization failed.";

        public const string AssociationDeleteCacheFailed =
            "Association was deleted from database, but cache synchronization failed.";
    }
}
