USE mysterybox;

DROP TABLE IF EXISTS `tb_reward_gold`;
CREATE TABLE `tb_reward_gold` (
  `user_id` INT(11) NOT NULL COMMENT '유저의 고유값',
  `gold` INT(11) NOT NULL COMMENT '아이템 ID',
  `reward_time` BIGINT(20) NOT NULL COMMENT '아이템 개수',
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '최근 업데이트 시간',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '최초 생성 시간',
  PRIMARY KEY (`user_id`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='골드 획득 정보';
