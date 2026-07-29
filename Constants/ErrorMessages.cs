namespace WebApplication2.Constants
{
    public static class ErrorMessages
    {
        public const string VenueNotFound =
            "Venue not found.";

        public const string VenueNameRequired =
            "Venue name is required.";

        public const string VenueCityRequired =
            "Venue city is required.";

        public const string VenueNameAlreadyExists =
            "An active venue with this name already exists.";

        public const string AnotherVenueNameAlreadyExists =
            "Another active venue with this name already exists.";

        public const string VenueHasActiveFloors =
            "The venue cannot be deleted because it contains active floors.";

        public const string FloorNotFound =
            "Floor not found.";

        public const string FloorNameRequired =
            "Floor name is required.";

        public const string AssignedVenueNotFound =
            "The assigned venue does not exist.";

        public const string FloorNameAlreadyExistsInVenue =
            "An active floor with this name already exists in the venue.";

        public const string FloorLevelAlreadyExistsInVenue =
            "An active floor with this level already exists in the venue.";

        public const string AnotherFloorNameAlreadyExistsInVenue =
            "Another active floor with this name already exists in the venue.";

        public const string AnotherFloorLevelAlreadyExistsInVenue =
            "Another active floor with this level already exists in the venue.";

        public const string FloorHasActiveZones =
            "The floor cannot be deleted because it contains active zones.";

        public const string ZoneNotFound =
            "Zone not found.";

        public const string ZoneNameRequired =
            "Zone name is required.";

        public const string AssignedFloorNotFound =
            "The assigned floor does not exist.";

        public const string ZoneNameAlreadyExistsOnFloor =
            "An active zone with this name already exists on the floor.";

        public const string AnotherZoneNameAlreadyExistsOnFloor =
            "Another active zone with this name already exists on the floor.";

        public const string PolygonRequiresThreePoints =
            "A zone polygon must contain at least three points.";

        public const string PolygonContainsDuplicateCoordinates =
            "A zone polygon cannot contain duplicate coordinates.";

        public const string PersonNotFound =
            "Person not found.";

        public const string PersonNameRequired =
            "Person name is required.";

        public const string PhoneNumberRequired =
            "Phone number is required.";

        public const string PersonNameAlreadyExists =
            "An active person with this name already exists.";

        public const string PersonAssociatedWithTag =
            "The person cannot be deleted because it is associated with a tag.";

        public const string InvalidPhoneNumber =
            "The phone number may contain only digits and an optional plus sign at the beginning.";

        public const string TagNotFound =
            "Tag not found.";

        public const string TagLabelRequired =
            "Tag label is required.";

        public const string TagMacAddressRequired =
            "Tag MAC address is required.";

        public const string TagMacAddressAlreadyExists =
            "An active tag with this MAC address already exists.";

        public const string TagAssociatedWithPerson =
            "The tag cannot be deleted because it is associated with a person.";

        public const string InvalidMacAddress =
            "The MAC address format is invalid.";

        public const string PersonAlreadyAssociated =
            "The person is already associated with a tag.";

        public const string TagAlreadyAssociated =
            "The tag is already associated with a person.";

        public const string AssociationNotFound =
            "The association between the person and tag was not found.";

        public const string FloorDoesNotBelongToVenue =
            "The floor does not belong to the selected venue.";

        public const string ZoneDoesNotBelongToFloor =
            "The zone does not belong to the selected floor.";
    }
}