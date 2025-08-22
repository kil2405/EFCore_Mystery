USE mysterybox;

DROP TABLE IF EXISTS `tb_item_reward`;
CREATE TABLE `tb_item_reward` (
  `user_id` INT(11) NOT NULL COMMENT '유저의 고유값',
  `appear_num` INT(11) NOT NULL COMMENT '아이템 보상 횟수',
  `count` BIGINT(20) NOT NULL COMMENT '최소 클릭 횟수',
  `is_reward` BIT(1) NOT NULL COMMENT '보상 여부 (true: 보상받음, false : 보상받지 않음)',
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '최근 업데이트 시간',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '최초 생성 시간',
  PRIMARY KEY (`user_id`, `appear_num`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='아이템 보상 정보';
