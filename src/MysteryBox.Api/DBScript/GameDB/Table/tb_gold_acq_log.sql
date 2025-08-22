USE mysterybox;

DROP TABLE IF EXISTS `tb_gold_acq_log`;
CREATE TABLE `tb_gold_acq_log` (
  `user_id` INT(11) NOT NULL COMMENT '유저의 고유값',
  `log_date` BIGINT(20) NOT NULL COMMENT '로그시간',
  `add_value` INT(11) NOT NULL COMMENT '획득한 아이템 ID',
  `total_value` BIGINT(20) NOT NULL COMMENT '총 보유 골드',
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '최근 업데이트 시간',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '최초 생성 시간',
  PRIMARY KEY (`user_id`, `log_date`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='아이템 획득 로그';
