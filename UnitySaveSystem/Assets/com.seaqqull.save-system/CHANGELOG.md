## [0.3.3] - 2023-05-31

### Fixed
- After ClearAll command in [World] all worlds stayed empty (should have been deleted)

## [0.3.2] - 2023-05-31

### Added
- CustomExporter
- CustomImporter
- CustomProvider

### Changed
- Path for [CreateAssetMenu] actions

## [0.3.1] - 2023-05-30

### Added
- Initialization via dedicated file [SaveSystemSetup]
- Updated initialization documentation

### Removed
- Redundant custom editor for [SaveManagerInitializerSO]

## [0.3.0] - 2023-05-23

### Added
- Different save types
- Continuous state backup
- Json importing/exporting
- Import/Export fabric
- SaveManager interfaces
- SaveManager initializer
- Save callback functions

### Fixed
- Location saved, after clear on its world was called & Save callback functions
- Sharing violation for non-existing file, when folder didn't exist

### Changed
- Instant-less [Item] linking
- Generalization of items stored in [Location]


## [0.2.0] - 2023-04-02

### Added
- Multiple [World]

### Fixed
- Missing snapshot timestamp


## [0.1.0] - 2023-03-25

### Added
- Single [World], multiple [Locations]
