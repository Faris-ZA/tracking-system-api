-- Sample data for tblpeople
INSERT INTO tblpeople (name, phone)
VALUES
('Ahmad Khalil', '0791234567'),
('Sara Ali', '0789876543'),
('Omar Hasan', '0775554433');


-- Sample data for tbltags
INSERT INTO tbltags (label, mac)
VALUES
('Ahmad Badge', 'AA:BB:CC:DD:EE:01'),
('Sara Badge', 'AA:BB:CC:DD:EE:02'),
('Omar Badge', 'AA:BB:CC:DD:EE:03');


-- Sample data for tblpeople_tag_association
INSERT INTO tblpeople_tag_association (people_id, tag_id)
VALUES
(1, 1),
(2, 2),
(3, 3);


-- Sample data for tblvenues
INSERT INTO tblvenues (name, city)
VALUES
('Main Headquarters', 'Amman'),
('North Branch', 'Irbid');


-- Sample data for tblfloors
INSERT INTO tblfloors (name, venue_id, level)
VALUES
('Ground Floor', 1, 0),
('First Floor', 1, 1),
('Ground Floor', 2, 0);


-- Sample data for tblzones
INSERT INTO tblzones (name, floor_id)
VALUES
('Reception', 1),
('Meeting Room', 1),
('Development Office', 2),
('Storage Area', 3);


-- Sample polygon points for Reception
INSERT INTO tblzones_polygon_points
(zone_id, point_index, x, y)
VALUES
(1, 1, 0, 0),
(1, 2, 100, 0),
(1, 3, 100, 80),
(1, 4, 0, 80);


-- Sample polygon points for Meeting Room
INSERT INTO tblzones_polygon_points
(zone_id, point_index, x, y)
VALUES
(2, 1, 120, 0),
(2, 2, 220, 0),
(2, 3, 220, 80),
(2, 4, 120, 80);


-- Sample polygon points for Development Office
INSERT INTO tblzones_polygon_points
(zone_id, point_index, x, y)
VALUES
(3, 1, 0, 100),
(3, 2, 200, 100),
(3, 3, 200, 220),
(3, 4, 0, 220);


-- Sample polygon points for Storage Area
INSERT INTO tblzones_polygon_points
(zone_id, point_index, x, y)
VALUES
(4, 1, 0, 0),
(4, 2, 150, 0),
(4, 3, 150, 120),
(4, 4, 0, 120);


-- Sample data for tbllast_position
INSERT INTO tbllast_position
(people_id, venue_id, floor_id, zone_id, x, y)
VALUES
(1, 1, 1, 1, 40, 30),
(2, 1, 1, 2, 170, 40),
(3, 1, 2, 3, 90, 150);


-- Sample data for tblposition_history
INSERT INTO tblposition_history
(people_id, venue_id, floor_id, zone_id, x, y, create_date)
VALUES
(1, 1, 1, 1, 20, 20, '2026-07-21 08:00:00'),
(1, 1, 1, 1, 40, 30, '2026-07-21 08:05:00'),
(2, 1, 1, 2, 150, 30, '2026-07-21 08:02:00'),
(2, 1, 1, 2, 170, 40, '2026-07-21 08:07:00'),
(3, 1, 2, 3, 70, 130, '2026-07-21 08:03:00'),
(3, 1, 2, 3, 90, 150, '2026-07-21 08:08:00');