export type EventType =
  | 'TwoPointShot'
  | 'ThreePointShot'
  | 'TwoPointMiss'
  | 'ThreePointMiss'
  | 'Assist'
  | 'ReboundOff'
  | 'ReboundDef'
  | 'Steal'
  | 'Block'
  | 'Turnover'
  | 'Foul'
  | 'OffensiveFoul'
  | 'TechnicalFoul'
  | 'FlagrantFoul'
  | 'FreeThrowMiss'
  | 'FreeThrowMade'
  | 'SubstituteIn'
  | 'SubstituteOut'
  | 'FoulReceived'
  | 'Timeout'
  | 'JumpBallWon';

export const SCORING_EVENT_TYPES: { value: EventType; label: string; requiresTeam?: boolean }[] = [
  { value: 'TwoPointShot', label: '2PT Made' },
  { value: 'TwoPointMiss', label: '2PT Miss' },
  { value: 'ThreePointShot', label: '3PT Made' },
  { value: 'ThreePointMiss', label: '3PT Miss' },
  { value: 'FreeThrowMade', label: 'FT Made' },
  { value: 'FreeThrowMiss', label: 'FT Miss' },
  { value: 'Assist', label: 'Assist' },
  { value: 'ReboundOff', label: 'Off. Rebound' },
  { value: 'ReboundDef', label: 'Def. Rebound' },
  { value: 'Steal', label: 'Steal' },
  { value: 'Block', label: 'Block' },
  { value: 'Turnover', label: 'Turnover' },
  { value: 'Foul', label: 'Foul' },
  { value: 'OffensiveFoul', label: 'Offensive Foul' },
  { value: 'TechnicalFoul', label: 'Technical Foul' },
  { value: 'FlagrantFoul', label: 'Flagrant Foul' },
];
