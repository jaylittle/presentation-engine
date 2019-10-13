UPDATE
  Version
SET
  Guid = hex(substr(Guid, 4, 1)) ||
  hex(substr(Guid, 3, 1)) ||
  hex(substr(Guid, 2, 1)) ||
  hex(substr(Guid, 1, 1)) || '-' ||
  hex(substr(Guid, 6, 1)) ||
  hex(substr(Guid, 5, 1)) || '-' ||
  hex(substr(Guid, 8, 1)) ||
  hex(substr(Guid, 7, 1)) || '-' ||
  hex(substr(Guid, 9, 2)) || '-' ||
  hex(substr(Guid, 11, 6))
WHERE
  typeof(Guid) == 'blob';

UPDATE
  Quote
SET
  Guid = hex(substr(Guid, 4, 1)) ||
  hex(substr(Guid, 3, 1)) ||
  hex(substr(Guid, 2, 1)) ||
  hex(substr(Guid, 1, 1)) || '-' ||
  hex(substr(Guid, 6, 1)) ||
  hex(substr(Guid, 5, 1)) || '-' ||
  hex(substr(Guid, 8, 1)) ||
  hex(substr(Guid, 7, 1)) || '-' ||
  hex(substr(Guid, 9, 2)) || '-' ||
  hex(substr(Guid, 11, 6))
WHERE
  typeof(Guid) == 'blob';